using System.Runtime.InteropServices;
using System.Security;
using Qml.Net.Internal;

namespace Qml.Net
{
    public class QtWebEngine
    {
        public static void Initialize()
        {
            Interop.QtWebEngine.Initialize();
        }
    }

    internal class QtWebEngineInterop
    {
        [NativeSymbol(Entrypoint = "qtwebebengine_initialize")]
        public InitializeDel Initialize { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void InitializeDel();
    }
}