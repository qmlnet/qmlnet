using System;
using AdvancedDLSupport;
using Qml.Net.Internal;
using Qml.Net.Qml;
using Qml.Net.Types;

namespace Qml.Net
{
    public static class Interop
    {
        static readonly CallbacksImpl DefaultCallbacks = new CallbacksImpl(new DefaultCallbacks());
        
        static Interop()
        {
            Environment.SetEnvironmentVariable("DYLD_LIBRARY_PATH", "/Users/pknopf/git/net-core-qml/src/native/build-QmlNet-Desktop_Qt_5_11_1_clang_64bit-Debug");
            Callbacks = NativeLibraryBuilder.Default.ActivateInterface<ICallbacksIterop>("QmlNet");
            NetTypeInfo = NativeLibraryBuilder.Default.ActivateInterface<INetTypeInfoInterop>("QmlNet");
            NetMethodInfo = NativeLibraryBuilder.Default.ActivateInterface<INetMethodInfoInterop>("QmlNet");
            NetPropertyInfo = NativeLibraryBuilder.Default.ActivateInterface<INetPropertyInfoInterop>("QmlNet");
            NetTypeManager = NativeLibraryBuilder.Default.ActivateInterface<INetTypeManagerInterop>("QmlNet");
            QGuiApplication = NativeLibraryBuilder.Default.ActivateInterface<IQGuiApplicationInterop>("QmlNet");
            QQmlApplicationEngine = NativeLibraryBuilder.Default.ActivateInterface<IQQmlApplicationEngine>("QmlNet");
            NetVariant = NativeLibraryBuilder.Default.ActivateInterface<INetVariantInterop>("QmlNet");
            NetReference = NativeLibraryBuilder.Default.ActivateInterface<INetReferenceInterop>("QmlNet");
            NetVariantList = NativeLibraryBuilder.Default.ActivateInterface<INetVariantListInterop>("QmlNet");
            NetTestHelper = NativeLibraryBuilder.Default.ActivateInterface<INetTestHelperInterop>("QmlNet");
            NetSignalInfo = NativeLibraryBuilder.Default.ActivateInterface<INetSignalInfoInterop>("QmlNet");
            QResource = NativeLibraryBuilder.Default.ActivateInterface<IQResourceInterop>("QmlNet");
            
            var cb = DefaultCallbacks.Callbacks();
            Callbacks.RegisterCallbacks(ref cb);
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
    }
}