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

        static Interop()
        {
            IPathResolver pathResolver = null;
            IPlatformLoader loader = null;

            if (Host.GetExportedSymbol != null)
            {
                // We are loading exported functions from the currently running executable.
                pathResolver = new Host.Loader();
                loader = new Host.Loader();
            }
            else
            {
                pathResolver = new DynamicLinkLibraryPathResolver();
                loader = PlatformLoaderBase.SelectPlatformLoader();
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    // This custom path resolver attempts to do a DllImport to get the path that .NET decides.
                    // It may load a special dll from a NuGet package.
                    pathResolver = new WindowsDllImportLibraryPathResolver(pathResolver);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    pathResolver = new MacDllImportLibraryPathResolver(pathResolver);
                }
                else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    pathResolver = new LinuxDllImportLibraryPathResolver(pathResolver);
                }
            }

            var result = pathResolver.Resolve("QmlNet");

            if (!result.IsSuccess)
            {
                throw new Exception("Unable to find the native Qml.Net library. Try calling \"RuntimeManager.DiscoverOrDownloadSuitableQtRuntime();\" in Program.Main()");
            }

            var library = loader.LoadLibrary(result.Path);
            Callbacks = LoadInteropType<CallbacksInterop>(library, loader);
            NetTypeInfo = LoadInteropType<NetTypeInfoInterop>(library, loader);
            NetJsValue = LoadInteropType<NetJsValueInterop>(library, loader);
            NetMethodInfo = LoadInteropType<NetMethodInfoInterop>(library, loader);
            NetPropertyInfo = LoadInteropType<NetPropertyInfoInterop>(library, loader);
            NetTypeManager = LoadInteropType<NetTypeManagerInterop>(library, loader);
            QCoreApplication = LoadInteropType<QCoreApplicationInterop>(library, loader);
            QQmlApplicationEngine = LoadInteropType<QQmlApplicationEngineInterop>(library, loader);
            NetVariant = LoadInteropType<NetVariantInterop>(library, loader);
            NetReference = LoadInteropType<NetReferenceInterop>(library, loader);
            NetVariantList = LoadInteropType<NetVariantListInterop>(library, loader);
            NetTestHelper = LoadInteropType<NetTestHelperInterop>(library, loader);
            NetSignalInfo = LoadInteropType<NetSignalInfoInterop>(library, loader);
            QResource = LoadInteropType<QResourceInterop>(library, loader);
            NetDelegate = LoadInteropType<NetDelegateInterop>(library, loader);
            QQuickStyle = LoadInteropType<QQuickStyleInterop>(library, loader);
            QtInterop = LoadInteropType<QtInterop>(library, loader);
            Utilities = LoadInteropType<UtilitiesInterop>(library, loader);
            QtWebEngine = LoadInteropType<QtWebEngineInterop>(library, loader);
            QTest = LoadInteropType<QTestInterop>(library, loader);
            NetQObject = LoadInteropType<NetQObjectInterop>(library, loader);
            NetQObjectSignalConnection = LoadInteropType<NetQObjectSignalConnectionInterop>(library, loader);
            QLocale = LoadInteropType<QLocaleInterop>(library, loader);

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