using System;
using System.Collections.Generic;
using System.Linq;
using Qml.Net.Internal.Types;

namespace Qml.Net.Internal
{
    internal static class InteropBehaviors
    {
        private static List<IQmlInteropBehavior> _QmlInteropBehaviors = new List<IQmlInteropBehavior>();
        public static IEnumerable<IQmlInteropBehavior> QmlInteropBehaviors => _QmlInteropBehaviors;

        private static IEnumerable<IQmlInteropBehavior> GetApplicableInteropBehaviors(Type forType)
        {
            return _QmlInteropBehaviors
                        .Where(b => b.IsApplicableFor(forType));
        }

        /// <summary> 
        /// Registers an additional Qml Interop behavior. This behavior only gets applied to types and instances created or registered after this behavior registration 
        /// </summary> 
        /// <param name="behavior"></param> 
        public static void RegisterQmlInteropBehavior(IQmlInteropBehavior behavior)
        {
            _QmlInteropBehaviors.Add(behavior);
        }

        public static void ClearQmlInteropBehaviors()
        {
            _QmlInteropBehaviors.Clear();
        }

        internal static void OnNetTypeInfoCreated(NetTypeInfo netTypeInfo, Type forType)
        {
            foreach (var behavior in GetApplicableInteropBehaviors(forType))
            {
                behavior.OnNetTypeInfoCreated(netTypeInfo, forType);
            }
        }

        internal static void OnNetReferenceCreatedForObject(object instance, UInt64 objectId)
        {
            if (instance == null)
            {
                return;
            }
            var type = instance.GetType();
            foreach (var behavior in GetApplicableInteropBehaviors(type))
            {
                behavior.OnNetReferenceCreatedForObject(instance, objectId);
            }
        }

        internal static void OnNetReferenceDeletedForObject(object instance, UInt64 objectId)
        {
            if (instance == null)
            {
                return;
            }
            var type = instance.GetType();
            foreach (var behavior in GetApplicableInteropBehaviors(type))
            {
                behavior.OnNetReferenceDeletedForObject(instance, objectId);
            }
        }
    }
}
