using Qml.Net.Internal.Types;

namespace Qml.Net
{
    public static class Qml
    {
        public static int RegisterType<T>(string uri, int versionMajor = 1, int versionMinor = 0)
        {
            return QQmlApplicationEngine.RegisterType(NetTypeManager.GetTypeInfo<T>(), uri, typeof(T).Name, versionMajor, versionMinor);
        }
    }
}