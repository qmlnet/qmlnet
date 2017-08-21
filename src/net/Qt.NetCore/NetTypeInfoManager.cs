using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qt.NetCore
{
    public partial class NetTypeInfoManager
    {
        public static NetTypeInfo GetTypeInfo<T>()
        {
            return GetTypeInfo(typeof(T));
        }

        public static NetTypeInfo GetTypeInfo(Type type)
        {
            return GetTypeInfo(type.FullName + ", " + type.Assembly.FullName);
        }
    }
}
