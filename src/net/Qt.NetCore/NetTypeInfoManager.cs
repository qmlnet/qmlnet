using System;

namespace Qt.NetCore
{
    public partial class NetTypeInfoManager
    {
        public static ITypeCreator TypeCreator { get; set; }

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
