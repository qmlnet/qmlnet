using System.Collections.Generic;

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

            var list = new List<T>();
            try
            {
                var length = (int)value.GetProperty("length");

                for (var i = 0; i < length; i++)
                {
                    var item = value.GetItemAtIndex(i);
                    list.Add((T)item);
                }

                return list;
            }
            catch
            {
                return null;
            }
        }
    }
}