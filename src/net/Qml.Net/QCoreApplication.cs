using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Qml.Net.Internal;
using Qml.Net.Internal.Qml;

namespace Qml.Net
{
    public class QCoreApplication : BaseDisposable
    {
        private readonly Queue<Action> _actionQueue = new Queue<Action>();
        private readonly SynchronizationContext _oldSynchronizationContext;
        private GCHandle _triggerHandle;

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
            TriggerDelegate triggerDelegate = Trigger;
            _triggerHandle = GCHandle.Alloc(triggerDelegate);

            Interop.QCoreApplication.AddTriggerCallback(Handle, Marshal.GetFunctionPointerForDelegate(triggerDelegate));

            _oldSynchronizationContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(new QtSynchronizationContext(this));
        }

        internal QCoreApplication(IntPtr existingApp)
            : base(CreateFromExisting(existingApp))
        {
            TriggerDelegate triggerDelegate = Trigger;
            _triggerHandle = GCHandle.Alloc(triggerDelegate);

            Interop.QCoreApplication.AddTriggerCallback(Handle, Marshal.GetFunctionPointerForDelegate(triggerDelegate));

            _oldSynchronizationContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(new QtSynchronizationContext(this));
        }

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

        public void Exit(int returnCode = 0)
        {
            Interop.QCoreApplication.Exit(returnCode);
        }

        public void Quit()
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

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            SynchronizationContext.SetSynchronizationContext(_oldSynchronizationContext);
            Interop.QCoreApplication.Destroy(ptr);
            _triggerHandle.Free();
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

        [NativeSymbol(Entrypoint = "qapp_getType")]
        public GetAppTypeDel GetAppType { get; set; }

        public delegate int GetAppTypeDel(IntPtr container, IntPtr rawPointer);

        public delegate void DestroyDel(IntPtr container);

        [NativeSymbol(Entrypoint = "qapp_exec")]
        public ExecDel Exec { get; set; }

        public delegate int ExecDel();

        [NativeSymbol(Entrypoint = "qapp_addTriggerCallback")]
        public AddTriggerCallbackDel AddTriggerCallback { get; set; }

        public delegate void AddTriggerCallbackDel(IntPtr app, IntPtr callback);

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