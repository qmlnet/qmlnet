using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Qml.Net.Internal;
using Qml.Net.Internal.Qml;

namespace Qml.Net
{
    public class QCoreApplication : BaseDisposable
    {
        private readonly Queue<Action> _actionQueue = new Queue<Action>();
        private SynchronizationContext _oldSynchronizationContext;
        private GCHandle _triggerHandle;
        private GCHandle _aboutToQuitHandle;
        private List<AboutToQuitEventHandler> _aboutToQuitEventHandlers = new List<AboutToQuitEventHandler>();
        private int _threadId;

        protected QCoreApplication(IntPtr handle, bool ownsHandle)
            : base(handle, ownsHandle)
        {
        }

        public QCoreApplication()
            : this(null)
        {
        }

        public QCoreApplication(string[] args, int flags = 0)
            : this(0, args, flags)
        {
        }

        internal QCoreApplication(int type, string[] args, int flags)
            : base(Create(type, args?.ToList(), flags))
        {
            Init();
        }

        internal QCoreApplication(IntPtr existingApp)
            : base(CreateFromExisting(existingApp))
        {
            Init();
        }

        private void Init()
        {
            _threadId = Environment.CurrentManagedThreadId;

            TriggerDelegate triggerDelegate = Trigger;
            _triggerHandle = GCHandle.Alloc(triggerDelegate);

            AboutToQuitDelegate aboutToQuitDelegate = AboutToQuitHandler;
            _aboutToQuitHandle = GCHandle.Alloc(aboutToQuitDelegate);

            var callbacks = new QCoreAppCallbacks
            {
                GuiThreadTrigger = Marshal.GetFunctionPointerForDelegate(triggerDelegate),
                AboutToQuitCb = Marshal.GetFunctionPointerForDelegate(aboutToQuitDelegate)
            };
            Interop.QCoreApplication.AddCallbacks(Handle, ref callbacks);

            _oldSynchronizationContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(new QtSynchronizationContext(this));
        }

        public bool IsMainThread => Environment.CurrentManagedThreadId == _threadId;

        public int Exec()
        {
            return Interop.QCoreApplication.Exec();
        }

        public void Dispatch(Action action)
        {
            lock (_actionQueue)
            {
                _actionQueue.Enqueue(action);
            }
            RequestTrigger();
        }

        public static void Exit(int returnCode = 0)
        {
            Interop.QCoreApplication.Exit(returnCode);
        }

        public static void Quit()
        {
            Exit();
        }

        private void RequestTrigger()
        {
            Interop.QCoreApplication.RequestTrigger(Handle);
        }

        internal IntPtr InternalPointer => Interop.QCoreApplication.InternalPointer(Handle);

        private void Trigger()
        {
            Action action;
            lock (_actionQueue)
            {
                action = _actionQueue.Dequeue();
            }
            action?.Invoke();
        }

        public static void ProcessEvents(QEventLoop.ProcessEventsFlag flags, TimeSpan? timeout = null)
        {
            if (timeout == null)
            {
                Interop.QCoreApplication.ProcessEvents((int)flags);
            }
            else
            {
                Interop.QCoreApplication.ProcessEventsWithTimeout((int)flags, (int)timeout.Value.TotalMilliseconds);
            }
        }

        private void AboutToQuitHandler()
        {
            List<AboutToQuitEventHandler> handlers;

            lock (_aboutToQuitEventHandlers)
            {
                handlers = _aboutToQuitEventHandlers.ToList();
            }

            if (handlers.Count == 0)
            {
                return;
            }

            var tasks = new List<Task>();

            foreach (var handler in handlers)
            {
                tasks.Add(handler.Invoke());
            }

            // Now, let's wait for all the tasks to completed.
            var exceptions = new List<Exception>();

            var completed = false;
            Task.Run(() =>
            {
                foreach (var task in tasks)
                {
                    try
                    {
                        task.Wait();
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }
                completed = true;
            });

            while (!completed)
            {
                ProcessEvents(QEventLoop.ProcessEventsFlag.EventLoopExec | QEventLoop.ProcessEventsFlag.WaitForMoreEvents);
            }

            if (exceptions.Count > 0)
            {
                throw new AggregateException("An exception(s) was thrown while running quit tasks.", exceptions);
            }
        }

        public delegate Task AboutToQuitEventHandler();

        public event AboutToQuitEventHandler AboutToQuit
        {
            add
            {
                lock (_aboutToQuitEventHandlers)
                {
                    _aboutToQuitEventHandlers.Add(value);
                }
            }

            remove
            {
                lock (_aboutToQuitEventHandlers)
                {
                    _aboutToQuitEventHandlers.Remove(value);
                }
            }
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            SynchronizationContext.SetSynchronizationContext(_oldSynchronizationContext);
            Interop.QCoreApplication.Destroy(ptr);
            _triggerHandle.Free();
            _aboutToQuitHandle.Free();
        }

        private static IntPtr CreateFromExisting(IntPtr app)
        {
            return Interop.QCoreApplication.FromExisting(app);
        }

        private static IntPtr Create(int type, List<string> args, int flags)
        {
            if (args == null)
            {
                args = new List<string>();
            }

            // By default, the argv[0] should be the process name.
            // .NET doesn't pass that name, but Qt should get it
            // since it does in a normal Qt environment.
            args.Insert(0, System.Diagnostics.Process.GetCurrentProcess().ProcessName);

            using (var strings = new NetVariantList())
            {
                foreach (var arg in args)
                {
                    using (var variant = new NetVariant())
                    {
                        variant.String = arg;
                        strings.Add(variant);
                    }
                }

                return Interop.QCoreApplication.Create(strings.Handle, flags, type);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void TriggerDelegate();

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void AboutToQuitDelegate();

        private class QtSynchronizationContext : SynchronizationContext
        {
            readonly QCoreApplication _app;

            public QtSynchronizationContext(QCoreApplication guiApp)
            {
                _app = guiApp;
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                _app.Dispatch(() => d.Invoke(state));
            }
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct QCoreAppCallbacks
    {
        public IntPtr GuiThreadTrigger;
        public IntPtr AboutToQuitCb;
    }

    internal class QCoreApplicationInterop
    {
        [NativeSymbol(Entrypoint = "qapp_fromExisting")]
        public FromExistingDel FromExisting { get; set; }

        public delegate IntPtr FromExistingDel(IntPtr rawPointer);

        [NativeSymbol(Entrypoint = "qapp_create")]
        public CreateDel Create { get; set; }

        public delegate IntPtr CreateDel(IntPtr args, int flags, int type);

        [NativeSymbol(Entrypoint = "qapp_destroy")]
        public DestroyDel Destroy { get; set; }

        public delegate void DestroyDel(IntPtr container);

        [NativeSymbol(Entrypoint = "qapp_getType")]
        public GetAppTypeDel GetAppType { get; set; }

        public delegate int GetAppTypeDel(IntPtr container, IntPtr rawPointer);

        [NativeSymbol(Entrypoint = "qapp_processEvents")]
        public ProcessEventsDel ProcessEvents { get; set; }

        public delegate void ProcessEventsDel(int flags);

        [NativeSymbol(Entrypoint = "qapp_processEventsWithTimeout")]
        public ProcessEventsWithTimeoutDel ProcessEventsWithTimeout { get; set; }

        public delegate void ProcessEventsWithTimeoutDel(int flags, int timeout);

        [NativeSymbol(Entrypoint = "qapp_exec")]
        public ExecDel Exec { get; set; }

        public delegate int ExecDel();

        [NativeSymbol(Entrypoint = "qapp_addCallbacks")]
        public AddCallbacksDel AddCallbacks { get; set; }

        public delegate void AddCallbacksDel(IntPtr app, ref QCoreAppCallbacks callbacks);

        [NativeSymbol(Entrypoint = "qapp_requestTrigger")]
        public RequestTriggerDel RequestTrigger { get; set; }

        public delegate void RequestTriggerDel(IntPtr app);

        [NativeSymbol(Entrypoint = "qapp_exit")]
        public ExitDel Exit { get; set; }

        public delegate void ExitDel(int returnCode);

        [NativeSymbol(Entrypoint = "qapp_internalPointer")]
        public InternalPointerDel InternalPointer { get; set; }

        public delegate IntPtr InternalPointerDel(IntPtr app);
    }
}