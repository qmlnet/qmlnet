using System;
using System.Runtime.InteropServices;
using System.Security;

namespace Qml.Net.Internal.Qml
{
    internal class NetTestHelper
    {
        public static bool RunQml(QQmlApplicationEngine qmlEngine, string qml, bool runEvents = false)
        {
            return Interop.NetTestHelper.RunQml(qmlEngine.Handle, qml, runEvents ? (byte) 1 : (byte) 0) == 1;
        }
    }

    internal class NetTestHelperInterop
    {
        [NativeSymbol(Entrypoint = "net_test_helper_runQml")]
        public RunQmlDel RunQml { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate byte RunQmlDel(IntPtr qmlEngine, [MarshalAs(UnmanagedType.LPWStr)]string qml, byte runEvents);
    }
}