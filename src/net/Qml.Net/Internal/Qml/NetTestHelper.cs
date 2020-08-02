using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security;

namespace Qml.Net.Internal.Qml
{
    internal class NetTestHelper
    {
        public static bool RunQml(QQmlApplicationEngine qmlEngine, string qml, bool runEvents = false, bool failOnQmlWarnings = true)
        {
            var warnings = new List<string>();
            var result = Interop.NetTestHelper.RunQml(qmlEngine.Handle, qml, runEvents ? (byte)1 : (byte)0, warnings.Add);
            if (warnings.Count > 0 && failOnQmlWarnings)
            {
                throw new Exception(string.Join("\n", warnings));
            }

            return result == 1;
        }
    }

    internal class NetTestHelperInterop
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Unicode)]
        public delegate void WarningDel(string text);

        [NativeSymbol(Entrypoint = "net_test_helper_runQml")]
        public RunQmlDel RunQml { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate byte RunQmlDel(IntPtr qmlEngine, [MarshalAs(UnmanagedType.LPWStr)] string qml, byte runEvents, WarningDel onWarning);
    }
}