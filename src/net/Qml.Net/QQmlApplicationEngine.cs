using System;
using System.Runtime.InteropServices;
using Qml.Net.Internal;
using Qml.Net.Internal.Behaviors;
using Qml.Net.Internal.Qml;
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

        public object GetContextProperty(string name)
        {
            var result = Interop.QQmlApplicationEngine.GetContextProperty(Handle, name);
            if (result == IntPtr.Zero)
            {
                return null;
            }

            using (var variant = new NetVariant(result))
            {
                object r = null;
                Helpers.Unpackvalue(ref r, variant);
                return r;
            }
        }

        public void SetContextProperty(string name, object value)
        {
            if (value == null)
            {
                Interop.QQmlApplicationEngine.SetContextProperty(Handle, name, IntPtr.Zero);
            }
            else
            {
                using (var variant = new NetVariant())
                {
                    Helpers.PackValue(value, variant);
                    Interop.QQmlApplicationEngine.SetContextProperty(Handle, name, variant.Handle);
                }
            }
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
    
    internal class QQmlApplicationEngineInterop
    {
        [NativeSymbol(Entrypoint = "qqmlapplicationengine_create")]
        public CreateDel Create { get; set; }
        public delegate IntPtr CreateDel(IntPtr existingEngine);
        
        [NativeSymbol(Entrypoint = "qqmlapplicationengine_destroy")]
        public DestroyDel Destroy { get; set; }
        public delegate void DestroyDel(IntPtr engine);

        [NativeSymbol(Entrypoint = "qqmlapplicationengine_load")]
        public LoadDel Load { get; set; }
        public delegate int LoadDel(IntPtr engine, [MarshalAs(UnmanagedType.LPWStr)]string path);

        [NativeSymbol(Entrypoint = "qqmlapplicationengine_loadData")]
        public LoadDataDel LoadData { get; set; }
        public delegate int LoadDataDel(IntPtr engine, [MarshalAs(UnmanagedType.LPWStr)]string path);

        [NativeSymbol(Entrypoint = "qqmlapplicationengine_registerType")]
        public RegisterTypeDel RegisterType { get; set; }
        public delegate int RegisterTypeDel(IntPtr type, [MarshalAs(UnmanagedType.LPWStr)]string uri, int versionMajor, int versionMinor, [MarshalAs(UnmanagedType.LPWStr)]string qmlName);
        
        [NativeSymbol(Entrypoint = "qqmlapplicationengine_registerSingletonTypeQml")]
        public RegisterSingletonTypeQmlDel RegisterSingletonTypeQml { get; set; }
        public delegate int RegisterSingletonTypeQmlDel([MarshalAs(UnmanagedType.LPWStr)]string url, [MarshalAs(UnmanagedType.LPWStr)]string uri, int versionMajor, int versionMinor, [MarshalAs(UnmanagedType.LPWStr)]string qmlName);

        [NativeSymbol(Entrypoint = "qqmlapplicationengine_registerSingletonTypeNet")]
        public RegisterSingletonTypeNetDel RegisterSingletonTypeNet { get; set; }
        public delegate int RegisterSingletonTypeNetDel(IntPtr type, [MarshalAs(UnmanagedType.LPWStr)]string uri, int versionMajor, int versionMinor, [MarshalAs(UnmanagedType.LPWStr)]string typeName);

        [NativeSymbol(Entrypoint = "qqmlapplicationengine_addImportPath")]
        public AddImportPathDel AddImportPath { get; set; }
        public delegate void AddImportPathDel(IntPtr engine, [MarshalAs(UnmanagedType.LPWStr)]string path);
        
        [NativeSymbol(Entrypoint = "qqmlapplicationengine_internalPointer")]
        public InternalPointerDel InternalPointer { get; set; }
        public delegate IntPtr InternalPointerDel(IntPtr app);
        
        [NativeSymbol(Entrypoint = "qqmlapplicationengine_getContextProperty")]
        public GetContextPropertyDel GetContextProperty { get; set; }
        public delegate IntPtr GetContextPropertyDel(IntPtr app, [MarshalAs(UnmanagedType.LPWStr)]string name);
        
        [NativeSymbol(Entrypoint = "qqmlapplicationengine_setContextProperty")]
        public SetContextPropertyDel SetContextProperty { get; set; }
        public delegate void SetContextPropertyDel(IntPtr app, [MarshalAs(UnmanagedType.LPWStr)]string path, IntPtr value);
    }
}