using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qt.NetCore.Internal;

namespace Qt.NetCore.Qml
{
    public class QGuiApplication : BaseDisposable
    {
        private Queue<Action> _actionQueue = new Queue<Action>();
        private GCHandle _triggerHandle; 
        
        public QGuiApplication()
            :base(Interop.QGuiApplication.Create())
        {
            TriggerDelegate triggerDelegate = Trigger;
            _triggerHandle = GCHandle.Alloc(triggerDelegate);
            Interop.QGuiApplication.AddTriggerCallback(Handle, Marshal.GetFunctionPointerForDelegate(triggerDelegate));
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
            Interop.QGuiApplication.Destroy(ptr);
            _triggerHandle.Free();
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void TriggerDelegate();
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
    }
    
}