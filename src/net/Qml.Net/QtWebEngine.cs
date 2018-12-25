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

        public delegate void InitializeDel();
    }
}