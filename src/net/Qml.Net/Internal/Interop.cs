using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qml.Net.Internal.Qml;
using Qml.Net.Internal.Types;

[assembly: InternalsVisibleTo("DLSupportDynamicAssembly")]

namespace Qml.Net.Internal
{
    internal static class Interop
    {
        static readonly CallbacksImpl DefaultCallbacks = new CallbacksImpl(new DefaultCallbacks());
        
        static Interop()
        {
            string pluginsDirectory = null;
            string qmlDirectory = null;

            ILibraryPathResolver pathResolver = 
                (ILibraryPathResolver)Activator.CreateInstance(Type.GetType("AdvancedDLSupport.DynamicLinkLibraryPathResolver, AdvancedDLSupport"), new object[] { true }); ;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                // This custom path resolver attempts to do a DllImport to get the path that .NET decides.
                // It may load a special dll from a NuGet package.
                pathResolver = new DllImportLibraryPathResolver(pathResolver);
                var resolveResult = ((DllImportLibraryPathResolver)pathResolver).Resolve("QmlNet");
                if(resolveResult.IsSuccess)
                {
                    var directory = Path.GetDirectoryName(resolveResult.Path);
                    // If this library as a plugins/qml directory below it, set it.
                    var potentialPlugisDirectory = Path.Combine(directory, "plugins");
                    if (Directory.Exists(potentialPlugisDirectory))
                    {
                        pluginsDirectory = potentialPlugisDirectory;
                    }
                    var potentialQmlDirectory = Path.Combine(directory, "qml");
                    if(Directory.Exists(potentialQmlDirectory))
                    {
                        qmlDirectory = potentialQmlDirectory;
                    }
                }
            }
            
            var builder = new NativeLibraryBuilder(pathResolver: pathResolver);
            
            var interop = builder.ActivateInterface<ICombined>("QmlNet");

            Callbacks = interop;
            NetTypeInfo = interop;
            NetMethodInfo = interop;
            NetPropertyInfo = interop;
            NetTypeManager = interop;
            QGuiApplication = interop;
            QQmlApplicationEngine = interop;
            NetVariant = interop;
            NetReference = interop;
            NetVariantList = interop;
            NetTestHelper = interop;
            NetSignalInfo = interop;
            QResource = interop;
            NetDelegate = interop;
            NetJsValue = interop;
            QQuickStyle = interop;
            QtInterop = interop;
            
            if(!string.IsNullOrEmpty(pluginsDirectory))
            {
                Qt.PutEnv("QT_PLUGIN_PATH", pluginsDirectory);
            }
            if(!string.IsNullOrEmpty(qmlDirectory))
            {
                Qt.PutEnv("QML2_IMPORT_PATH", qmlDirectory);
            }

            var cb = DefaultCallbacks.Callbacks();
            Callbacks.RegisterCallbacks(ref cb);
        }

        // ReSharper disable PossibleInterfaceMemberAmbiguity
        // ReSharper disable MemberCanBePrivate.Global
        internal interface ICombined :
        // ReSharper restore MemberCanBePrivate.Global
        // ReSharper restore PossibleInterfaceMemberAmbiguity
            ICallbacksIterop,
            INetTypeInfoInterop,
            INetMethodInfoInterop,
            INetPropertyInfoInterop,
            INetTypeManagerInterop,
            IQGuiApplicationInterop,
            IQQmlApplicationEngine,
            INetVariantInterop,
            INetReferenceInterop,
            INetVariantListInterop,
            INetTestHelperInterop,
            INetSignalInfoInterop,
            IQResourceInterop,
            INetDelegateInterop,
            INetJsValueInterop,
            IQQuickStyleInterop,
            IQtInterop
        {

        }
        
        public static ICallbacksIterop Callbacks { get; }

        public static INetTypeInfoInterop NetTypeInfo { get; }
        
        public static INetMethodInfoInterop NetMethodInfo { get; }
        
        public static INetPropertyInfoInterop NetPropertyInfo { get; }
        
        public static INetTypeManagerInterop NetTypeManager { get; }
        
        public static IQGuiApplicationInterop QGuiApplication { get; }
        
        public static IQQmlApplicationEngine QQmlApplicationEngine { get; }
        
        public static INetVariantInterop NetVariant { get; }
        
        public static INetReferenceInterop NetReference { get; }
        
        public static INetVariantListInterop NetVariantList { get; }
        
        public static INetTestHelperInterop NetTestHelper { get; }
        
        public static INetSignalInfoInterop NetSignalInfo { get; }
        
        public static IQResourceInterop QResource { get; }
        
        public static INetDelegateInterop NetDelegate { get; }
        
        public static INetJsValueInterop NetJsValue { get; }
        
        public static IQQuickStyleInterop QQuickStyle { get; }

        public static IQtInterop QtInterop { get; }
    }
}