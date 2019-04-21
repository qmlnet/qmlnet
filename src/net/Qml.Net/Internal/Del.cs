using System;
using Qml.Net.Internal.Qml;

namespace Qml.Net.Internal
{
    internal class Del
    {
        public event Action<NetVariantList> Invoked;

        public void Raise(NetVariantList parameters)
        {
            var handler = Invoked;
            handler?.Invoke(parameters);
        }
    }
}