using System.Runtime.InteropServices;
using AdvancedDLSupport;

namespace Qml.Net.Qml
{
    public class QResource
    {
        public static bool RegisterResource(string rccFileName, string resourceRoot = null)
        {
            return Interop.QResource.RegisterResource(rccFileName, resourceRoot);
        }
    }
    
    public interface IQResourceInterop
    {
        [NativeSymbol(Entrypoint = "qresource_registerResource")]
        bool RegisterResource([MarshalAs(UnmanagedType.LPWStr)]string rccFileName, [MarshalAs(UnmanagedType.LPWStr)]string resourceRoot);
    }
}