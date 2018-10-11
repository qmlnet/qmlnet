using System.Collections.Generic;
using System.Linq;
using Qml.Net.Internal.Qml;

namespace Qml.Net.Extensions
{
    public static class INetJsValueExtensions
    {
        public static IEnumerable<T> AsEnumerable<T>(this INetJsValue value)
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
                var lengthProp = (NetVariant)value.GetProperty("length");
                var length = lengthProp.Int;
                
                for (var i = 0; i < length; i++)
                {
                    var item = (NetVariant)value.GetItemAtIndex(i);
                    if(typeof(T) == typeof(int))
                    {
                        object result = item.Int;
                        list.Add((T)result);
                    }
                    else
                    {
                        object result = item.String;
                        list.Add((T)result);
                    }
                }

                return list.AsEnumerable();
            }
            catch
            {
                return null;
            }
        }
    }
}