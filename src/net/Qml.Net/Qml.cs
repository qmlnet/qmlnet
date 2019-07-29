using System;
using System.Runtime.InteropServices;
using Qml.Net.Aot;
using Qml.Net.Internal;
using Qml.Net.Internal.Types;

namespace Qml.Net
{
    public static class Qml
    {
        public static int RegisterType<T>(string uri, int versionMajor = 1, int versionMinor = 0)
        {
            return RegisterType(typeof(T), typeof(T).Name, uri, versionMajor, versionMinor);
        }

        public static int RegisterType(Type type, string qmlName, string uri, int versionMajor = 1, int versionMinor = 0)
        {
            if (AotTypes.TryGetAotTypeId(type, out int aotTypeId))
            {
                return QQmlApplicationEngine.RegisterType(aotTypeId, uri, qmlName, versionMajor, versionMinor);
            }

            using (var typeInfo = NetTypeManager.GetTypeInfo(type))
            {
                return QQmlApplicationEngine.RegisterType(typeInfo, uri, qmlName, versionMajor, versionMinor);
            }
        }

        public static int RegisterSingletonType(string url, string qmlName, string uri, int versionMajor = 1, int versionMinor = 0)
        {
            return Interop.QQmlApplicationEngine.RegisterSingletonTypeQml(url, uri, versionMajor, versionMinor, qmlName);
        }

        public static int RegisterSingletonType<T>(string uri, int versionMajor = 1, int versionMinor = 0)
        {
            return RegisterSingletonType(typeof(T), typeof(T).Name, uri, versionMajor, versionMinor);
        }

        public static int RegisterSingletonType(Type type, string qmlName, string uri, int versionMajor = 1, int versionMinor = 0)
        {
            if (AotTypes.TryGetAotTypeId(type, out int aotTypeId))
            {
                return Interop.QQmlApplicationEngine.RegisterSingletonTypeNet(IntPtr.Zero, aotTypeId, uri, versionMajor, versionMinor, qmlName);
            }

            using (var typeInfo = NetTypeManager.GetTypeInfo(type))
            {
                return Interop.QQmlApplicationEngine.RegisterSingletonTypeNet(typeInfo.Handle, -1, uri, versionMajor, versionMinor, qmlName);
            }
        }
    }
}