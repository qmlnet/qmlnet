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
    }
    
    internal interface IQResourceInterop
    {
        [NativeSymbol(Entrypoint = "qresource_registerResource")]
        bool RegisterResource([MarshalAs(UnmanagedType.LPWStr)]string rccFileName, [MarshalAs(UnmanagedType.LPWStr)]string resourceRoot);
    }
}