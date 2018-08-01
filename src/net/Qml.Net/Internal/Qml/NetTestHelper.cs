using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;

namespace Qml.Net.Internal.Qml
{
    internal class NetTestHelper
    {
        public static void RunQml(QQmlApplicationEngine qmlEngine, string qml)
        {
            Interop.NetTestHelper.RunQml(qmlEngine.Handle, qml);
        }
    }
    
    internal interface INetTestHelperInterop
    {
        [NativeSymbol(Entrypoint = "net_test_helper_runQml")]
        void RunQml(IntPtr qmlEngine, [MarshalAs(UnmanagedType.LPWStr), CallerFree]string qml);
    }
}