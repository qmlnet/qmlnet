using System.Runtime.InteropServices;
using Qml.Net.Internal;
using Qml.Net.Internal.Types;

namespace Qml.Net
{
    public static class Qml
    {
        public static int RegisterType<T>(string uri, int versionMajor = 1, int versionMinor = 0)
        {
            return QQmlApplicationEngine.RegisterType(NetTypeManager.GetTypeInfo<T>(), uri, typeof(T).Name, versionMajor, versionMinor);
        }

        public static int RegisterSingletonType(string url, string uri, int versionMajor, int versionMinor, string qmlName)
        {
            return Interop.QQmlApplicationEngine.RegisterSingletonTypeQml(url, uri, versionMajor, versionMinor, qmlName);
        }
        
        public static int RegisterSingletonType<T>(string uri, int versionMajor = 1, int versionMinor = 0)
        {
            using (var type = NetTypeManager.GetTypeInfo<T>())
            {
                return Interop.QQmlApplicationEngine.RegisterSingletonTypeNet(type.Handle, uri, versionMajor, versionMinor, typeof(T).Name);
            }
        }
    }
}