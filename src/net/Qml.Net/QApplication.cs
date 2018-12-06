using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using AdvancedDLSupport;
using Qml.Net.Internal;
using Qml.Net.Internal.Qml;

namespace Qml.Net
{
    public sealed class QApplication : BaseDisposable
    {
        readonly Queue<Action> _actionQueue = new Queue<Action>();
        GCHandle _triggerHandle;
        readonly SynchronizationContext _oldSynchronizationContext;

        public QApplication()
            :this(null)
        {
            
        }
        
        public QApplication(string[] args)
            :base(Create(args?.ToList()))
        {
            TriggerDelegate triggerDelegate = Trigger;
            _triggerHandle = GCHandle.Alloc(triggerDelegate);
            
            Interop.QApplication.AddTriggerCallback(Handle, Marshal.GetFunctionPointerForDelegate(triggerDelegate));
            
            _oldSynchronizationContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(new QtSynchronizationContext(this));
        }

        internal QApplication(IntPtr existingApp)
            :base(CreateFromExisting(existingApp))
        {
            TriggerDelegate triggerDelegate = Trigger;
            _triggerHandle = GCHandle.Alloc(triggerDelegate);
            
            Interop.QApplication.AddTriggerCallback(Handle, Marshal.GetFunctionPointerForDelegate(triggerDelegate));
            
            _oldSynchronizationContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(new QtSynchronizationContext(this));
        }

        public int Exec()
        {
            return Interop.QApplication.Exec(Handle);
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
            Interop.QApplication.Exit(Handle, returnCode);
        }

        public void Quit()
        {
            Exit();
        }

        private void RequestTrigger()
        {
            Interop.QApplication.RequestTrigger(Handle);
        }

        internal IntPtr InternalPointer => Interop.QApplication.InternalPointer(Handle);

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
            Interop.QApplication.Destroy(ptr);
            _triggerHandle.Free();
        }

        private static IntPtr CreateFromExisting(IntPtr app)
        {
            return Interop.QApplication.Create(IntPtr.Zero, app);
        }

        private static IntPtr Create(List<string> args)
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
                return Interop.QApplication.Create(strings.Handle, IntPtr.Zero);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void TriggerDelegate();
        
        private class QtSynchronizationContext : SynchronizationContext
        {
            readonly QApplication _app;

            public QtSynchronizationContext(QApplication app)
            {
                _app = app;
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                _app.Dispatch(() => d.Invoke(state));
            }
        }
    }
    
    internal interface IQApplicationInterop
    {
        [NativeSymbol(Entrypoint = "qapplication_create")]
        IntPtr Create(IntPtr args, IntPtr existingApp);
        [NativeSymbol(Entrypoint = "qapplication_destroy")]
        void Destroy(IntPtr app);

        [NativeSymbol(Entrypoint = "qapplication_exec")]
        int Exec(IntPtr app);
        [NativeSymbol(Entrypoint = "qapplication_addTriggerCallback")]
        void AddTriggerCallback(IntPtr app, IntPtr callback);
        [NativeSymbol(Entrypoint = "qapplication_requestTrigger")]
        void RequestTrigger(IntPtr app);
        [NativeSymbol(Entrypoint = "qapplication_exit")]
        void Exit(IntPtr app, int returnCode);
        [NativeSymbol(Entrypoint = "qapplication_internalPointer")]
        IntPtr InternalPointer(IntPtr app);
    }
}