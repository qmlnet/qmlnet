using System;
using Qml.Net.Internal.Types;

namespace Qml.Net.Internal.Behaviors
{
    // All properties have a default notify signal.
    // This is so that when we read properties in QML,
    // we don't get errors with "NON-NOTIFY PROPERTY BOUND
    public class AutoGenerateNotifySignalsBehavior : IQmlInteropBehavior
    {
        public int Priority => 1000;

        public bool IsApplicableFor(Type type)
        {
            return true;
        }

        void IQmlInteropBehavior.OnNetTypeInfoCreated(NetTypeInfo netTypeInfo, Type forType)
        {
            if (!IsApplicableFor(forType))
            {
                return;
            }
            for (var i = 0; i < netTypeInfo.PropertyCount; i++)
            {
                int? existingSignalIndex = null;

                var property = netTypeInfo.GetProperty(i);
                if (property.NotifySignal != null)
                {
                    // In this case some other behavior or the user has already set up a notify signal for this property.
                    // We don't want to destroy that.
                    continue;
                }
                var signalName = $"dynamic__{property.Name}Changed";

                // Check if this signal already has been registered.
                for (var signalIndex = 0; signalIndex < netTypeInfo.SignalCount; signalIndex++)
                {
                    var signal = netTypeInfo.GetSignal(signalIndex);
                    if (string.Equals(signalName, signal.Name))
                    {
                        existingSignalIndex = signalIndex;
                        break;
                    }
                }
                if (existingSignalIndex.HasValue)
                {
                    // Signal for this property is already existent but not registered (we check that above).
                    property.NotifySignal = netTypeInfo.GetSignal(existingSignalIndex.Value);
                    continue;
                }

                // Create a new signal and link it to the property.
                var notifySignalInfo = new NetSignalInfo(netTypeInfo, signalName);
                netTypeInfo.AddSignal(notifySignalInfo);
                property.NotifySignal = notifySignalInfo;
            }
        }

        public void OnObjectEntersNative(object instance, ulong objectId)
        {
            // NOOP
        }

        public void OnObjectLeavesNative(object instance, ulong objectId)
        {
            // NOOP
        }
    }
}