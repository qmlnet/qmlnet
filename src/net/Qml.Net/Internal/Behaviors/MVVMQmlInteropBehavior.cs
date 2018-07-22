using Qml.Net.Internal.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Qml.Net.Internal.Behaviors
{
    internal class MVVMQmlInteropBehavior : IQmlInteropBehavior
    {
        public bool IsApplicableFor(Type type)
        {
            return typeof(INotifyPropertyChanged).IsAssignableFrom(type);
        }

        private static Dictionary<UInt64, uint> ObjectNetReferenceCount = new Dictionary<ulong, uint>();

        public void OnNetReferenceCreatedForObject(object instance, UInt64 objectId)
        {
            if (instance == null)
            {
                return;
            }
            lock (this)
            {
                bool isFirst = false;
                if (ObjectNetReferenceCount.ContainsKey(objectId))
                {
                    ObjectNetReferenceCount[objectId]++;
                }
                else
                {
                    isFirst = true;
                    ObjectNetReferenceCount.Add(objectId, 1);
                }
                if (isFirst)
                {
                    var castedInstance = instance as INotifyPropertyChanged;
                    if (castedInstance == null)
                    {
                        //invalid type 
                        return;
                    }
                    castedInstance.PropertyChanged += PropertyChangedHandler;
                }
            }
        }

        public void OnNetReferenceDeletedForObject(object instance, UInt64 objectId)
        {
            if (instance == null)
            {
                return;
            }
            lock (this)
            {
                if (!ObjectNetReferenceCount.ContainsKey(objectId))
                {
                    //This should not happen! This would mean that a NetReference is deleted that hasn't been counted before
                    return;
                }
                ObjectNetReferenceCount[objectId]--;
                //check if this had been the last NetReference
                if (ObjectNetReferenceCount[objectId] == 0)
                {
                    var castedInstance = instance as INotifyPropertyChanged;
                    if (castedInstance == null)
                    {
                        //invalid type 
                        return;
                    }

                    castedInstance.PropertyChanged -= PropertyChangedHandler;
                }
            }
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            //fire signal according to the property that got changed 
            string signalName = null;
            signalName = CalculateSignalSignatureFromPropertyName(e.PropertyName);
            if (signalName != null)
            {
                Signals.ActivateSignal(sender, signalName);
            }
        }

        private string CalculateSignalSignatureFromPropertyName(string propertyName)
        {
            return $"{propertyName}Changed";
        }

        public void OnNetTypeInfoCreated(NetTypeInfo netTypeInfo, Type forType)
        {
            if (!IsApplicableFor(forType))
            {
                return;
            }
            for (uint i = 0; i < netTypeInfo.PropertyCount; i++)
            {
                uint? existingSignalIndex = null;

                var property = netTypeInfo.GetProperty(i);
                var signalName = CalculateSignalSignatureFromPropertyName(property.Name);
                //check if this signal already has been registered
                for(uint signalIndex = 0; signalIndex < netTypeInfo.SignalCount; signalIndex++)
                {
                    var signal = netTypeInfo.GetSignal(signalIndex);
                    if(string.Equals(signalName, signal.Name))
                    {
                        existingSignalIndex = signalIndex;
                        break;
                    }
                }
                if(existingSignalIndex.HasValue)
                {
                    //signal for this property is already existent
                    
                    //check if the property is linked to it
                    if(property.NotifySignal == null)
                    {
                        property.NotifySignal = netTypeInfo.GetSignal(existingSignalIndex.Value);
                    }
                    continue;
                }
                //create a new signal and link it to the property
                var notifySignalInfo = new NetSignalInfo(netTypeInfo, signalName);
                netTypeInfo.AddSignal(notifySignalInfo);
                property.NotifySignal = notifySignalInfo;
            }
        }
    }
}
