using System;
using System.Runtime.InteropServices;
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

        public void Load(string path)
        {
            Interop.QQmlApplicationEngine.Load(Handle, path);
        }

        public static int RegisterType<T>(int versionMajor = 1, int versionMinor = 0)
        {
            return Interop.QQmlApplicationEngine.RegisterType(typeof(T).AssemblyQualifiedName, versionMajor, versionMinor, typeof(T).Name);
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

        [NativeSymbol(Entrypoint = "qqmlapplicationengine_load")]
        int Load(IntPtr engine, [MarshalAs(UnmanagedType.LPWStr)]string path);
        
        [NativeSymbol(Entrypoint = "qqmlapplicationengine_registerType")]
        int RegisterType([MarshalAs(UnmanagedType.LPWStr)]string typeName, int versionMajor, int versionMinor, [MarshalAs(UnmanagedType.LPWStr)]string componentName);
    }

}