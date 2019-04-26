using System.Runtime.InteropServices;
using System.Security;
using Qml.Net.Internal;

namespace Qml.Net
{
    public static class QResource
    {
        public static bool RegisterResource(string rccFileName, string resourceRoot = null)
        {
            return Internal.Interop.QResource.RegisterResource(rccFileName, resourceRoot) == 1;
        }

        public static bool UnregisterResource(string rccFileName, string resourceRoot = null)
        {
            return Internal.Interop.QResource.UnregisterResource(rccFileName, resourceRoot) == 1;
        }
    }

    internal class QResourceInterop
    {
        [NativeSymbol(Entrypoint = "qresource_registerResource")]
        public RegisterResourceDel RegisterResource { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate byte RegisterResourceDel([MarshalAs(UnmanagedType.LPWStr)]string rccFileName, [MarshalAs(UnmanagedType.LPWStr)]string resourceRoot);

        [NativeSymbol(Entrypoint = "qresource_unregisterResource")]
        public UnregisterResourceDel UnregisterResource { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate byte UnregisterResourceDel([MarshalAs(UnmanagedType.LPWStr)]string rccFileName, [MarshalAs(UnmanagedType.LPWStr)]string resourceRoot);
    }
}