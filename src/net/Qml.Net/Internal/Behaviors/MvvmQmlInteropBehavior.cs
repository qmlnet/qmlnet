using Qml.Net.Internal.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Qml.Net.Internal.Behaviors
{
    internal class MvvmQmlInteropBehavior : IQmlInteropBehavior
    {
        private class MvvmPropertyInfo
        {
            public MvvmPropertyInfo(string name, string signalName)
            {
                Name = name;
                SignalName = signalName;
            }
            
            // ReSharper disable once MemberCanBePrivate.Local
            // ReSharper disable once UnusedAutoPropertyAccessor.Local
            public string Name { get; }
            public string SignalName { get; }
        }

        private class MvvmTypeInfo
        {
            private readonly Dictionary<string, MvvmPropertyInfo> _propertyInfos = new Dictionary<string, MvvmPropertyInfo>();
            
            // ReSharper disable once UnusedMember.Local
            public Type Type { get; set; }

            public void AddPropertyInfo(string propertyName, string signalName)
            {
                _propertyInfos.Add(propertyName, new MvvmPropertyInfo(propertyName, signalName));
            }

            public string GetPropertySignalName(string propertyName)
            {
                return !_propertyInfos.ContainsKey(propertyName) ? null : _propertyInfos[propertyName].SignalName;
            }
        }

        private static readonly Dictionary<Type, MvvmTypeInfo> TypeInfos = new Dictionary<Type, MvvmTypeInfo>();
        
        public bool IsApplicableFor(Type type)
        {
            return typeof(INotifyPropertyChanged).IsAssignableFrom(type);
        }

        public void OnObjectEntersNative(object instance, UInt64 objectId)
        {
            if (instance == null)
            {
                return;
            }
            if (!(instance is INotifyPropertyChanged castedInstance))
            {
                //invalid type 
                return;
            }
            castedInstance.PropertyChanged += PropertyChangedHandler;
        }

        public void OnObjectLeavesNative(object instance, UInt64 objectId)
        {
            if (instance == null)
            {
                return;
            }
            if (!(instance is INotifyPropertyChanged castedInstance))
            {
                //invalid type 
                return;
            }

            castedInstance.PropertyChanged -= PropertyChangedHandler;
        }

        private void PropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            if (sender == null)
            {
                return;
            }
            //fire signal according to the property that got changed
            var type = sender.GetType();
            if (TypeInfos.TryGetValue(type, out var typeInfo))
            {
                var signalName = typeInfo.GetPropertySignalName(e.PropertyName);
                if (signalName != null)
                {
                    sender.ActivateSignal(signalName);
                }
            }
        }

        private static string CalculateSignalNameFromPropertyName(string propertyName)
        {
            var result = $"{propertyName}Changed";
            if (!char.IsLower(result[0]))
            {
                return char.ToLower(result[0]) + result.Substring(1);
            }
            return result;
        }

        public void OnNetTypeInfoCreated(NetTypeInfo netTypeInfo, Type forType)
        {
            if (!IsApplicableFor(forType))
            {
                return;
            }
            var mvvmTypeInfo = new MvvmTypeInfo();
            TypeInfos.Add(forType, mvvmTypeInfo);
            for (var i = 0; i < netTypeInfo.PropertyCount; i++)
            {
                int? existingSignalIndex = null;

                var property = netTypeInfo.GetProperty(i);
                if (property.NotifySignal != null)
                {
                    //in this case some other behavior or the user has already set up a notify signal for this property
                    //we don't want to destroy that
                    mvvmTypeInfo.AddPropertyInfo(property.Name, property.NotifySignal.Name);
                    continue;
                }
                var signalName = CalculateSignalNameFromPropertyName(property.Name);
                mvvmTypeInfo.AddPropertyInfo(property.Name, signalName);
                //check if this signal already has been registered
                for(var signalIndex = 0; signalIndex < netTypeInfo.SignalCount; signalIndex++)
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
                    //signal for this property is already existent but not registered (we check that above)                    
                    property.NotifySignal = netTypeInfo.GetSignal(existingSignalIndex.Value);
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
