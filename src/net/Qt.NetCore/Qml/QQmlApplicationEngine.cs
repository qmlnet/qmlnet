using System;
using AdvancedDLSupport;
using Qt.NetCore.Internal;

namespace Qt.NetCore.Qml
{
    public class QQmlApplicationEngine : BaseDisposable
    {
        public QQmlApplicationEngine()
            :base(Interop.QQmlApplicationEngine.Create())
        {
            
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.QQmlApplicationEngine.Destroy(ptr);
        }
    }
    
    public interface IQQmlApplicationEngine
    {
        [NativeSymbol(Entrypoint = "qqmlapplicationengine_create")]
        IntPtr Create();
        [NativeSymbol(Entrypoint = "qqmlapplicationengine_destroy")]
        void Destroy(IntPtr engine);
    }

}