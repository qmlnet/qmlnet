using System.Runtime.InteropServices;
using System.Security;
using Qml.Net.Internal;

namespace Qml.Net
{
    public class QQuickStyle
    {
        public static void SetFallbackStyle(string style)
        {
            Interop.QQuickStyle.SetFallbackStyle(style);
        }

        public static void SetStyle(string style)
        {
            Interop.QQuickStyle.SetStyle(style);
        }
    }

    internal class QQuickStyleInterop
    {
        [NativeSymbol(Entrypoint = "qquickstyle_setFallbackStyle")]
        public SetFallbackStyleDel SetFallbackStyle { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetFallbackStyleDel([MarshalAs(UnmanagedType.LPWStr)]string style);

        [NativeSymbol(Entrypoint = "qquickstyle_setStyle")]
        public SetStyleDel SetStyle { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void SetStyleDel([MarshalAs(UnmanagedType.LPWStr)]string style);
    }
}