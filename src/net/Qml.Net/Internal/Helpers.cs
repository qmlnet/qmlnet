using System;
using Qml.Net.Qml;
using Qml.Net.Types;

namespace Qml.Net.Internal
{
    internal static class Helpers
    {
        public static bool IsPrimitive(Type type)
        {
            if (type.Namespace == "System") return true;
            if (type.Namespace == "System.Threading.Tasks") return true;
            return false;
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
                if (type == typeof(bool))
                    destination.Bool = (bool)source;
                else if(type == typeof(char))
                    destination.Char = (char)source;
                else if(type == typeof(double))
                    destination.Double = (double)source;
                else if (type == typeof(int))
                    destination.Int = (int)source;
                else if(type == typeof(uint))
                    destination.UInt = (uint)source;
                else if (type == typeof(string))
                    destination.String = (string)source;
                else if(type == typeof(DateTime))
                    destination.DateTime = (DateTime)source;
                else
                {
                    destination.Instance = NetReference.CreateForObject(source);
                }
            }
        }

        public static  void Unpackvalue(ref object destination, NetVariant source)
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
                default:
                    throw new Exception("Unsupported variant type.");
            }
        }
    }
}