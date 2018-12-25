using System;

namespace Qml.Net.Internal
{
    public class NativeSymbolAttribute : Attribute
    {
        public string Entrypoint { get; set; }
    }
}
