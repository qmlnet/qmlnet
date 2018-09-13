using AdvancedDLSupport;
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
    
    internal interface IQtWebEngine
    {
        [NativeSymbol(Entrypoint = "qtwebebengine_initialize")]
        void Initialize();
    }
}