using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Qml.Net.Internal
{
    internal class ObjectSignalCollection : IDisposable
    {
        public ObjectSignalCollection()
        {
            Delegates = new Dictionary<string, List<Delegate>>();
        }

        public void Dispose()
        {
            
        }
        
        public Dictionary<string, List<Delegate>> Delegates { get; }
    }
    
    internal static class ObjectSignals
    {
        private static readonly ConditionalWeakTable<object, ObjectSignalCollection> Signals 
            = new ConditionalWeakTable<object, ObjectSignalCollection>();

        public static void AttachDelegateToSignal(this object obj, string signal, Delegate del)
        {
            var signals = Signals.GetOrCreateValue(obj);
            if (signals.Delegates.ContainsKey(signal))
            {
                signals.Delegates[signal].Add(del);
            }
            signals.Delegates.Add(signal, new List<Delegate>{del});
        }
        
        public static List<Delegate> GetAttachedDelegates(this object obj, string signal)
        {
            if (Signals.TryGetValue(obj, out var signalCollection))
            {
                return signalCollection.Delegates.ContainsKey(signal)
                    ? signalCollection.Delegates[signal]
                    : null;
            }

            return null;
        }
    }
}