using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading;
using AdvancedDLSupport;
using Qt.NetCore.Internal;

namespace Qt.NetCore.Qml
{
    public class QGuiApplication : BaseDisposable
    {
        readonly Queue<Action> _actionQueue = new Queue<Action>();
        GCHandle _triggerHandle;
        readonly SynchronizationContext _oldSynchronizationContext;
        
        public QGuiApplication()
            :base(Interop.QGuiApplication.Create())
        {
            TriggerDelegate triggerDelegate = Trigger;
            _triggerHandle = GCHandle.Alloc(triggerDelegate);
            
            Interop.QGuiApplication.AddTriggerCallback(Handle, Marshal.GetFunctionPointerForDelegate(triggerDelegate));
            
            _oldSynchronizationContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(new QtSynchronizationContext(this));
        }

        public int Exec()
        {
            return Interop.QGuiApplication.Exec(Handle);
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
            Interop.QGuiApplication.Exit(Handle, returnCode);
        }

        public void Quit()
        {
            Exit();
        }

        private void RequestTrigger()
        {
            Interop.QGuiApplication.RequestTrigger(Handle);
        }

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
            Interop.QGuiApplication.Destroy(ptr);
            _triggerHandle.Free();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void TriggerDelegate();
        
        private class QtSynchronizationContext : SynchronizationContext
        {
            readonly QGuiApplication _guiApp;

            public QtSynchronizationContext(QGuiApplication guiApp)
            {
                _guiApp = guiApp;
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                _guiApp.Dispatch(() => d.Invoke(state));
            }
        }
    }
    
    public interface IQGuiApplicationInterop
    {
        [NativeSymbol(Entrypoint = "qguiapplication_create")]
        IntPtr Create();
        [NativeSymbol(Entrypoint = "qguiapplication_destroy")]
        void Destroy(IntPtr app);

        [NativeSymbol(Entrypoint = "qguiapplication_exec")]
        int Exec(IntPtr app);
        [NativeSymbol(Entrypoint = "qguiapplication_addTriggerCallback")]
        void AddTriggerCallback(IntPtr app, IntPtr callback);
        [NativeSymbol(Entrypoint = "qguiapplication_requestTrigger")]
        void RequestTrigger(IntPtr app);
        [NativeSymbol(Entrypoint = "qguiapplication_exit")]
        void Exit(IntPtr app, int returnCode);
    }
    
}