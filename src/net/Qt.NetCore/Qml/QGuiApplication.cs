using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qt.NetCore.Internal;

namespace Qt.NetCore.Qml
{
    public class QGuiApplication : BaseDisposable
    {
        public QGuiApplication()
            :base(Interop.QGuiApplication.Create())
        {
            
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.QGuiApplication.Destroy(ptr);
        }
    }
    
    public interface IQGuiApplicationInterop
    {
        [NativeSymbol(Entrypoint = "qguiapplication_create")]
        IntPtr Create();
        [NativeSymbol(Entrypoint = "qguiapplication_destroy")]
        void Destroy(IntPtr app);
    }
    
}