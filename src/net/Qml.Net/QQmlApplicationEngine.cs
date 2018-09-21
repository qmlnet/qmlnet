using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qml.Net.Internal;
using Qml.Net.Internal.Behaviors;
using Qml.Net.Internal.Types;

namespace Qml.Net
{
    public sealed class QQmlApplicationEngine : BaseDisposable
    {
        public QQmlApplicationEngine()
            :base(Interop.QQmlApplicationEngine.Create(IntPtr.Zero))
        {
            
        }

        internal QQmlApplicationEngine(IntPtr existingEngine)
            :base(Interop.QQmlApplicationEngine.Create(existingEngine))
        {
            
        }

        public void Load(string path)
        {
            Interop.QQmlApplicationEngine.Load(Handle, path);
        }

        public void LoadData(string data)
        {
            Interop.QQmlApplicationEngine.LoadData(Handle, data);
        }
        
        public void AddImportPath(string path)
        {
            Interop.QQmlApplicationEngine.AddImportPath(Handle, path);
        }
        
        internal IntPtr InternalPointer => Interop.QQmlApplicationEngine.InternalPointer(Handle);

        [Obsolete("Use Qml.RegisterType<T>() instead.", true)]
        public static int RegisterType<T>(string uri, int versionMajor = 1, int versionMinor = 0)
        {
            return RegisterType(NetTypeManager.GetTypeInfo<T>(), uri, typeof(T).Name, versionMajor, versionMinor);
        }

        internal static int RegisterType(NetTypeInfo type, string uri, string qmlName, int versionMajor = 1, int versionMinor = 0)
        {
            return Interop.QQmlApplicationEngine.RegisterType(type.Handle, uri, versionMajor, versionMinor, qmlName);
        }

        /// <summary>
        /// Activates the MVVM behavior.
        /// This Behavior automatically connects INotifyPropertyChanged instances with appropriate signals on the QML side
        /// and triggers those signals whenever the PropertyChanged event of the INotifyPropertyChanged instances is triggered.
        /// 
        /// Call this before any INotifyPropertyChanged type is registered! 
        /// Otherwise the behavior might not include
        /// INotifyPropertyChanged types that were registered (implicitly or explicitly) before this call
        /// </summary>
        public static void ActivateMVVMBehavior()
        {
            InteropBehaviors.RegisterQmlInteropBehavior(new MvvmQmlInteropBehavior(), false);
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.QQmlApplicationEngine.Destroy(ptr);
        }
    }
    
    internal interface IQQmlApplicationEngine
    {
        [NativeSymbol(Entrypoint = "qqmlapplicationengine_create")]
        IntPtr Create(IntPtr existingEngine);
        [NativeSymbol(Entrypoint = "qqmlapplicationengine_destroy")]
        void Destroy(IntPtr engine);

        [NativeSymbol(Entrypoint = "qqmlapplicationengine_load")]
        int Load(IntPtr engine, [MarshalAs(UnmanagedType.LPWStr), CallerFree]string path);

        [NativeSymbol(Entrypoint = "qqmlapplicationengine_loadData")]
        int LoadData(IntPtr engine, [MarshalAs(UnmanagedType.LPWStr), CallerFree]string path);

        [NativeSymbol(Entrypoint = "qqmlapplicationengine_registerType")]
        int RegisterType(IntPtr type, [MarshalAs(UnmanagedType.LPWStr), CallerFree]string uri, int versionMajor, int versionMinor, [MarshalAs(UnmanagedType.LPWStr), CallerFree]string qmlName);

        [NativeSymbol(Entrypoint = "qqmlapplicationengine_addImportPath")]
        void AddImportPath(IntPtr engine, [MarshalAs(UnmanagedType.LPWStr), CallerFree]string path);
        
        [NativeSymbol(Entrypoint = "qqmlapplicationengine_internalPointer")]
        IntPtr InternalPointer(IntPtr app);
    }
}