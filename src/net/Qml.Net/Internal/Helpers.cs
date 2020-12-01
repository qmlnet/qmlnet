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

        public static bool ShouldBeHiddenFromQml(Type type)
        {
            // we don't mirror QmlNetQuickPaintedItem properties and methods into QML because they (partly) mirror the QML type (QQuickPaintedItem)
            if (type == typeof(QmlNetQuickPaintedItem))
            {
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
            else if (type == typeof(byte[]))
                destination.ByteArray = (byte[])source;
            else if (type == typeof(DateTimeOffset))
                destination.DateTime = ((DateTimeOffset)source).DateTime;
            else if (typeof(INetJsValue).IsAssignableFrom(type))
                destination.JsValue = ((NetJsValue.NetJsValueDynamic)source).JsValue;
            else if (typeof(INetQObject).IsAssignableFrom(type))
                destination.QObject = ((NetQObject.NetQObjectDynamic)source).QObject;
            else
            {
                if (type.IsEnum)
                {
                    Pack(source, destination, type.GetEnumUnderlyingType());
                }
                else
                {
                    if (source == null)
                    {
                        destination.SetNull();
                    }
                    else
                    {
                        destination.Instance = NetReference.CreateForObject(source);
                    }
                }
            }
        }

        public static void PackValue(object source, NetVariant destination)
        {
            if (source == null)
            {
                destination.SetNull();
            }
            else
            {
                var type = source.GetType();
                Pack(source, destination, type);
            }
        }

        public static void Unpackvalue(ref object destination, NetVariant source)
        {
            destination = source.AsObject();
        }
    }
}