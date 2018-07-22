using System;
using System.Runtime.CompilerServices;
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
            var interop = NativeLibraryBuilder.Default.ActivateInterface<Combined>("QmlNet");

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

            var cb = DefaultCallbacks.Callbacks();
            Callbacks.RegisterCallbacks(ref cb);
        }

        internal interface Combined :
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
            IQResourceInterop
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
    }
}