using System.Runtime.InteropServices;
using AdvancedDLSupport;

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
    
    internal interface IQResourceInterop
    {
        [NativeSymbol(Entrypoint = "qresource_registerResource")]
        bool RegisterResource([MarshalAs(UnmanagedType.LPWStr), CallerFree]string rccFileName, [MarshalAs(UnmanagedType.LPWStr), CallerFree]string resourceRoot);
        [NativeSymbol(Entrypoint = "qresource_unregisterResource")]
        bool UnregisterResource([MarshalAs(UnmanagedType.LPWStr), CallerFree]string rccFileName, [MarshalAs(UnmanagedType.LPWStr), CallerFree]string resourceRoot);
    }
}