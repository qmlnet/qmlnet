using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
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

        /// <summary>
        /// Activates the signal that has been attached to the given property using NotifySignalAttribute
        /// </summary>
        /// <param name="instance">object instance having the property the changed signal has to be activated for</param>
        /// <param name="propertyName">
        ///     name of the property. Gets automatically filled with the caller member name.
        ///     So calling it directly out of the property that is tied to the signal doesn't need to set this parameter explicitly.</param>
        /// <returns>true when the signal has been activated, otherwise false</returns>
        public static bool ActivateNotifySignal(this object instance, [CallerMemberName] string propertyName = "")
        {
            if (instance == null)
                return false;
            var propertyInfo =
                instance.GetType().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public);
            if (propertyInfo == null)
                return false;
            var notifySignalAttributes = propertyInfo.GetCustomAttributes().OfType<NotifySignalAttribute>();
            var notifySignalAttribute = notifySignalAttributes.FirstOrDefault();
            if (notifySignalAttribute == null)
                return false;
            var signalName = notifySignalAttribute.Name;
            if (string.IsNullOrEmpty(signalName))
            {
                signalName = $"{propertyInfo.Name}Changed";
                signalName = char.ToLower(signalName[0]) + signalName.Substring(1);
            }
            return ActivateSignal(instance, signalName);
        }

        public static void AttachToSignal(this object instance, string signalName, System.Delegate del)
        {
            instance.AttachDelegateToSignal(signalName, del);
        }
    }
}