using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Qt.NetCore
{
    public static class Utils
    {
        public static void PackValue(object source, NetVariant destination, bool registerForNotifyPropertyChangedSignalPassing)
        {
            if(destination == null)
            {
                return;
            }
            if (source == null)
            {
                destination.Clear();
            }
            else
            {
                var type = source.GetType();
                if (type == typeof(bool))
                    destination.SetBool((bool)source);
                else if (type == typeof(char))
                    destination.SetChar((char)source);
                else if (type == typeof(double))
                    destination.SetDouble((double)source);
                else if (type == typeof(int))
                    destination.SetInt((int)source);
                else if (type == typeof(uint))
                    destination.SetUInt((uint)source);
                else if (type == typeof(string))
                    destination.SetString((string)source);
                else if (type == typeof(DateTime))
                    destination.SetDateTime((DateTime)source);
                else
                {
                    GCHandle createdHandle;
                    destination.SetNetInstance(NetTypeInfoManager.WrapCreatedInstance(
                        GCHandle.ToIntPtr(createdHandle = GCHandle.Alloc(source)),
                        NetTypeInfoManager.GetTypeInfo(GetUnproxiedType(type))));
                    Utils.TryAttachNotifyPropertyChanged(source, createdHandle);
                }
            }
        }

        public static void Unpackvalue(ref object destination, NetVariant source)
        {
            switch (source.GetVariantType())
            {
                case NetVariantTypeEnum.NetVariantTypeEnum_Invalid:
                    destination = null;
                    break;
                case NetVariantTypeEnum.NetVariantTypeEnum_Bool:
                    destination = source.GetBool();
                    break;
                case NetVariantTypeEnum.NetVariantTypeEnum_Char:
                    destination = source.GetChar();
                    break;
                case NetVariantTypeEnum.NetVariantTypeEnum_Int:
                    destination = source.GetInt();
                    break;
                case NetVariantTypeEnum.NetVariantTypeEnum_UInt:
                    destination = source.GetUInt();
                    break;
                case NetVariantTypeEnum.NetVariantTypeEnum_Double:
                    destination = source.GetDouble();
                    break;
                case NetVariantTypeEnum.NetVariantTypeEnum_String:
                    destination = source.GetString();
                    break;
                case NetVariantTypeEnum.NetVariantTypeEnum_DateTime:
                    destination = source.GetDateTime();
                    break;
                case NetVariantTypeEnum.NetVariantTypeEnum_Object:
                    var netInstance = source.GetNetInstance();
                    var gcHandle = (GCHandle)netInstance.GetGCHandle();
                    destination = gcHandle.Target;
                    break;
                default:
                    throw new Exception("Unsupported variant type.");
            }
        }

        public static string CalculatePropertyChangedSignalName(string propertyName)
        {
            //onPropertyNameChanged
            //first letter is capital
            StringBuilder result = new StringBuilder(propertyName.Length + /* Changed() */ + 9);
            for(int i=0; i<propertyName.Length; i++)
            {
                if(i == 0)
                {
                    result.Append(propertyName[i].ToString().ToUpper());
                }
                else
                {
                    result.Append(propertyName[i]);
                }
            }
            result.Append("Changed()");

            return result.ToString();
        }

        private static Type GetUnproxiedType(Type type)
        {
            if (type.Namespace == "Castle.Proxies")
                return type.BaseType;

            return type;
        }

        private static List<WeakReference<INotifyPropertyChanged>> _RegisteredNotifyPropertyChangeObjects = new List<WeakReference<INotifyPropertyChanged>>();

        public static void TryAttachNotifyPropertyChanged(object netInstance, GCHandle handle)
        {
            if (netInstance == null)
            {
                return;
            }
            if (netInstance is INotifyPropertyChanged)
            {
                //clean list
                _RegisteredNotifyPropertyChangeObjects.RemoveAll(
                    entry =>
                    {
                        return !entry.TryGetTarget(out INotifyPropertyChanged dummyTarget);
                    });
                var alreadyRegistered = _RegisteredNotifyPropertyChangeObjects.Any(
                    entry =>
                    {
                        if (!entry.TryGetTarget(out INotifyPropertyChanged target))
                        {
                            return false;
                        }
                        return object.ReferenceEquals(target, netInstance);
                    });
                if (alreadyRegistered)
                {
                    return;
                }
                var castedNetInstance = netInstance as INotifyPropertyChanged;
                _RegisteredNotifyPropertyChangeObjects.Add(new WeakReference<INotifyPropertyChanged>(castedNetInstance));
                castedNetInstance.PropertyChanged += new PropertyChangedEventHandler(
                    (sender, e) =>
                    {
                        QQmlApplicationEngine.TryActivateSignal(handle, Utils.CalculatePropertyChangedSignalName(e.PropertyName));
                    });
            }
        }
    }
}
