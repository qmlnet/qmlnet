using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;

namespace Qml.Net.Qml
{
    public class NetTestHelper
    {
        public static void RunQml(QQmlApplicationEngine qmlEngine, string qml)
        {
            Interop.NetTestHelper.RunQml(qmlEngine.Handle, qml);
        }
    }
    
    public interface INetTestHelperInterop
    {
        [NativeSymbol(Entrypoint = "net_test_helper_runQml")]
        void RunQml(IntPtr qmlEngine, [MarshalAs(UnmanagedType.LPWStr)]string qml);
    }
}