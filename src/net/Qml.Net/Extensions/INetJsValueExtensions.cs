using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;

namespace Qml.Net.Extensions
{
    public static class NetJsValueExtensions
    {
        public static List<T> AsList<T>(this INetJsValue value)
        {
            if (value == null || !value.IsArray)
            {
                return null;
            }

            if (typeof(T) != typeof(int) && typeof(T) != typeof(string))
            {
                // Only enumerables of int and string are currently supported
                return null;
            }

            var destinationConverter = TypeDescriptor.GetConverter(typeof(T));

            var list = new List<T>();

            var length = (int)value.GetProperty("length");

            for (var i = 0; i < length; i++)
            {
                var item = value.GetItemAtIndex(i);
                list.Add((T)destinationConverter.ConvertFrom(null, CultureInfo.InvariantCulture, item));
            }

            return list;
        }
    }
}