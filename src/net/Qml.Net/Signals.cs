using Qml.Net.Internal;
using Qml.Net.Internal.Types;

namespace Qml.Net
{
    public static class Signals
    {
        public static bool ActivateSignal(this object instance, string signalName, params object[] args)
        {
            var existing = NetReference.CreateForObject(instance, false /*Ignore if not tagged*/);
            if (existing != null && existing.ActivateSignal(signalName, args))
            {
                // This means the the instance was alive in Qml, and the signal was
                // activated. We don't have to store in signals stored in .NET
                // because when the instance is sent to Qml, any delegates associated
                // with the .NET object are auto-attached to correct signals.
                // So, nothing to do here, because all the .NET delegates were raised
                // elsewhere!
                return true;
            }

            var signals = instance.GetAttachedDelegates(signalName);
            
            if (signals == null || signals.Count <= 0)
            {
                return false;
            }
            
            foreach (var del in signals)
            {
                del.DynamicInvoke(args);
            }
            
            return true;
        }

        public static void AttachToSignal(this object instance, string signalName, System.Delegate del)
        {
            instance.AttachDelegateToSignal(signalName, del);
        }
    }
}