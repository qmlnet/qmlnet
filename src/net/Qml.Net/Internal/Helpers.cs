using System;
using Qml.Net.Internal.Qml;
using Qml.Net.Internal.Types;

namespace Qml.Net.Internal
{
    internal static class Helpers
    {
        public static bool IsPrimitive(Type type)
        {
            if (type.IsArray)
            {
                return false;
            }

            if (type.Namespace == "System")
            {
                if (type.Name == "Exception")
                {
                    return false;
                }

                if (type.Name == "Array")
                {
                    return false;
                }

                return true;
            }

            return false;
        }

        public static void Pack(object source, NetVariant destination, Type type)
        {
            if (type == typeof(bool))
                destination.Bool = (bool)source;
            else if (type == typeof(char))
                destination.Char = (char)source;
            else if (type == typeof(float))
                destination.Float = (float)source;
            else if (type == typeof(double))
                destination.Double = (double)source;
            else if (type == typeof(int))
                destination.Int = (int)source;
            else if (type == typeof(uint))
                destination.UInt = (uint)source;
            else if (type == typeof(long))
                destination.Long = (long)source;
            else if (type == typeof(ulong))
                destination.ULong = (ulong)source;
            else if (type == typeof(string))
                destination.String = (string)source;
            else if (type == typeof(DateTimeOffset))
                destination.DateTime = ((DateTimeOffset)source).DateTime;
            else if (typeof(INetJsValue).IsAssignableFrom(type))
                destination.JsValue = ((NetJsValue.NetJsValueDynamic)source).JsValue;
            else
            {
                if (type.IsEnum)
                {
                    Pack(source, destination, type.GetEnumUnderlyingType());
                }
                else
                {
                    destination.Instance = NetReference.CreateForObject(source);
                }
            }
        }

        public static void PackValue(object source, NetVariant destination)
        {
            if (source == null)
            {
                destination.Clear();
            }
            else
            {
                var type = source.GetType();
                Pack(source, destination, type);
            }
        }

        public static void Unpackvalue(ref object destination, NetVariant source)
        {
            switch (source.VariantType)
            {
                case NetVariantType.Invalid:
                    destination = null;
                    break;
                case NetVariantType.Bool:
                    destination = source.Bool;
                    break;
                case NetVariantType.Char:
                    destination = source.Char;
                    break;
                case NetVariantType.Int:
                    destination = source.Int;
                    break;
                case NetVariantType.UInt:
                    destination = source.UInt;
                    break;
                case NetVariantType.Long:
                    destination = source.Long;
                    break;
                case NetVariantType.ULong:
                    destination = source.ULong;
                    break;
                case NetVariantType.Float:
                    destination = source.Float;
                    break;
                case NetVariantType.Double:
                    destination = source.Double;
                    break;
                case NetVariantType.String:
                    destination = source.String;
                    break;
                case NetVariantType.DateTime:
                    destination = source.DateTime;
                    break;
                case NetVariantType.Object:
                    destination = source.Instance.Instance;
                    break;
                case NetVariantType.JsValue:
                    destination = source.JsValue.AsDynamic();
                    break;
                default:
                    throw new Exception("Unsupported variant type.");
            }
        }
    }
}