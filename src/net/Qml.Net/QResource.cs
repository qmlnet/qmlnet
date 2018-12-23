using System.Runtime.InteropServices;
using Qml.Net.Internal;

namespace Qml.Net
{
    public static class QResource
    {
        public static bool RegisterResource(string rccFileName, string resourceRoot = null)
        {
            return Internal.Interop.QResource.RegisterResource(rccFileName, resourceRoot);
        }

        public static bool UnregisterResource(string rccFileName, string resourceRoot = null)
        {
            return Internal.Interop.QResource.UnregisterResource(rccFileName, resourceRoot);
        }
    }
    
    internal class QResourceInterop
    {
        [NativeSymbol(Entrypoint = "qresource_registerResource")]
        public RegisterResourceDel RegisterResource { get; set; }
        public delegate bool RegisterResourceDel([MarshalAs(UnmanagedType.LPWStr)]string rccFileName, [MarshalAs(UnmanagedType.LPWStr)]string resourceRoot);
        [NativeSymbol(Entrypoint = "qresource_unregisterResource")]
        public UnregisterResourceDel UnregisterResource { get; set; }
        public delegate bool UnregisterResourceDel([MarshalAs(UnmanagedType.LPWStr)]string rccFileName, [MarshalAs(UnmanagedType.LPWStr)]string resourceRoot);
    }
}