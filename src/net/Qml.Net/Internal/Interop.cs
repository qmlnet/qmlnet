using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using NetNativeLibLoader.Loader;
using NetNativeLibLoader.PathResolver;
using Qml.Net.Internal.Qml;
using Qml.Net.Internal.Types;

namespace Qml.Net.Internal
{
    internal static class Interop
    {
        static readonly CallbacksImpl DefaultCallbacks = new CallbacksImpl(new DefaultCallbacks());

        internal static IPathResolver Resolver;
        internal static IPlatformLoader Loader;
        internal static IntPtr Library;
        
        static Interop()
        {
            Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", "/home/pknopf/git/qmlnet/src/native/build-QmlNet-Local-Debug");
            
            if (Host.GetExportedSymbol != null)
            {
                // We are loading exported functions from the currently running executable.
                Resolver = new Host.Loader();
                Loader = new Host.Loader();
            }
            else
            {
                Resolver = new DynamicLinkLibraryPathResolver();
                Loader = PlatformLoaderBase.SelectPlatformLoader();
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // This custom path resolver attempts to do a DllImport to get the path that .NET decides.
                    // It may load a special dll from a NuGet package.
                    Resolver = new WindowsDllImportLibraryPathResolver(Resolver);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    Resolver = new MacDllImportLibraryPathResolver(Resolver);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    Resolver = new LinuxDllImportLibraryPathResolver(Resolver);
                }
            }

            var result = Resolver.Resolve(QmlNetConfig.NativeLibName);

            if (!result.IsSuccess)
            {
                throw new Exception("Unable to find the native Qml.Net library. Try calling \"RuntimeManager.DiscoverOrDownloadSuitableQtRuntime();\" in Program.Main()");
            }

            Library = Loader.LoadLibrary(result.Path);
            Callbacks = LoadInteropType<CallbacksInterop>(Library, Loader);
            NetTypeInfo = LoadInteropType<NetTypeInfoInterop>(Library, Loader);
            NetJsValue = LoadInteropType<NetJsValueInterop>(Library, Loader);
            NetMethodInfo = LoadInteropType<NetMethodInfoInterop>(Library, Loader);
            NetPropertyInfo = LoadInteropType<NetPropertyInfoInterop>(Library, Loader);
            NetTypeManager = LoadInteropType<NetTypeManagerInterop>(Library, Loader);
            QCoreApplication = LoadInteropType<QCoreApplicationInterop>(Library, Loader);
            QQmlApplicationEngine = LoadInteropType<QQmlApplicationEngineInterop>(Library, Loader);
            NetVariant = LoadInteropType<NetVariantInterop>(Library, Loader);
            NetReference = LoadInteropType<NetReferenceInterop>(Library, Loader);
            NetVariantList = LoadInteropType<NetVariantListInterop>(Library, Loader);
            NetTestHelper = LoadInteropType<NetTestHelperInterop>(Library, Loader);
            NetSignalInfo = LoadInteropType<NetSignalInfoInterop>(Library, Loader);
            QResource = LoadInteropType<QResourceInterop>(Library, Loader);
            NetDelegate = LoadInteropType<NetDelegateInterop>(Library, Loader);
            QQuickStyle = LoadInteropType<QQuickStyleInterop>(Library, Loader);
            QtInterop = LoadInteropType<QtInterop>(Library, Loader);
            Utilities = LoadInteropType<UtilitiesInterop>(Library, Loader);
            QtWebEngine = LoadInteropType<QtWebEngineInterop>(Library, Loader);
            QTest = LoadInteropType<QTestInterop>(Library, Loader);
            NetQObject = LoadInteropType<NetQObjectInterop>(Library, Loader);
            NetQObjectSignalConnection = LoadInteropType<NetQObjectSignalConnectionInterop>(Library, Loader);
            QLocale = LoadInteropType<QLocaleInterop>(Library, Loader);

            // RuntimeManager.ConfigureRuntimeDirectory may set these environment variables.
            // However, they only really work when called with Qt.PutEnv.
            Qt.PutEnv("QT_PLUGIN_PATH", Environment.GetEnvironmentVariable("QT_PLUGIN_PATH"));
            Qt.PutEnv("QML2_IMPORT_PATH", Environment.GetEnvironmentVariable("QML2_IMPORT_PATH"));

            var cb = DefaultCallbacks.Callbacks();
            Callbacks.RegisterCallbacks(ref cb);
        }

        public static CallbacksInterop Callbacks { get; }

        public static NetTypeInfoInterop NetTypeInfo { get; }

        public static NetMethodInfoInterop NetMethodInfo { get; }

        public static NetPropertyInfoInterop NetPropertyInfo { get; }

        public static NetTypeManagerInterop NetTypeManager { get; }

        public static QCoreApplicationInterop QCoreApplication { get; }

        public static QQmlApplicationEngineInterop QQmlApplicationEngine { get; }

        public static NetVariantInterop NetVariant { get; }

        public static NetReferenceInterop NetReference { get; }

        public static NetVariantListInterop NetVariantList { get; }

        public static NetTestHelperInterop NetTestHelper { get; }

        public static NetSignalInfoInterop NetSignalInfo { get; }

        public static QResourceInterop QResource { get; }

        public static NetDelegateInterop NetDelegate { get; }

        public static NetJsValueInterop NetJsValue { get; }

        public static QQuickStyleInterop QQuickStyle { get; }

        public static QtInterop QtInterop { get; }

        public static UtilitiesInterop Utilities { get; }

        public static QtWebEngineInterop QtWebEngine { get; }

        public static QTestInterop QTest { get; }
        
        public static NetQObjectInterop NetQObject { get; }
        
        public static NetQObjectSignalConnectionInterop NetQObjectSignalConnection { get; }
        
        public static QLocaleInterop QLocale { get; set; }

        private static T LoadInteropType<T>(IntPtr library, IPlatformLoader loader)
            where T : new()
        {
            var result = new T();
            LoadDelegates(result, library, loader);
            return result;
        }

        private static void LoadDelegates(object o, IntPtr library, NetNativeLibLoader.Loader.IPlatformLoader loader)
        {
            foreach (var property in o.GetType().GetProperties())
            {
                var entryName = property.GetCustomAttributes().OfType<NativeSymbolAttribute>().First().Entrypoint;
                var symbol = loader.LoadSymbol(library, entryName);
                property.SetValue(o, Marshal.GetDelegateForFunctionPointer(symbol, property.PropertyType));
            }
        }
    }
}