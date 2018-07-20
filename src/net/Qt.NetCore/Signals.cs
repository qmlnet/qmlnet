using Qt.NetCore.Types;

namespace Qt.NetCore
{
    public static class Signals
    {
        public static void ActivateSignal(this object instance, string signalName, params object[] args)
        {
            var existing = NetInstance.GetForObject(instance, false /*don't create one if doesn't exist*/);
            existing?.ActivateSignal(signalName, args);
        }
    }
}