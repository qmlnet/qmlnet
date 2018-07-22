using Qml.Net.Internal.Types;

namespace Qml.Net
{
    public static class Signals
    {
        public static bool ActivateSignal(this object instance, string signalName, params object[] args)
        {
            var existing = NetReference.CreateForObject(instance);
            return existing != null && existing.ActivateSignal(signalName, args);
        }
    }
}