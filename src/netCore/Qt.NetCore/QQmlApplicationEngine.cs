using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Qt.NetCore
{
    public partial class QQmlApplicationEngine
    {
        public static int RegisterType<T>(string uri, int versionMajor, int versionMinor, string qmlName = null)
        {
            if (string.IsNullOrEmpty(qmlName))
            {
                qmlName = typeof(T).Name;
            }
            return QtNetCoreQml.registerNetType(
                typeof(T).FullName + ", " + typeof(T).Assembly.FullName,
                uri,
                versionMajor,
                versionMinor,
                qmlName);
        }

        public static void ActivateSignal(GCHandle handle, string signalName, params object[] args)
        {
            var netTypeInfo = NetTypeInfoManager.GetTypeInfo(handle.Target.GetType());
            QtNetCoreQml.activateSignal(GCHandle.ToIntPtr(handle), netTypeInfo.GetFullTypeName(), signalName, PackVariantArgs(args));
        }

        public static bool TryActivateSignal(GCHandle handle, string signalName, params object[] args)
        {
            if(handle == null)
            {
                return false;
            }
            var handleTarget = handle.Target;
            if(handleTarget == null)
            {
                return false;
            }
            var netTypeInfo = NetTypeInfoManager.GetTypeInfo(handleTarget.GetType());
            if(netTypeInfo == null)
            {
                return false;
            }
            return QtNetCoreQml.tryActivateSignal(GCHandle.ToIntPtr(handle), netTypeInfo.GetFullTypeName(), signalName, PackVariantArgs(args));
        }

        private static NetVariantVector PackVariantArgs(object[] args)
        {
            NetVariantVector result = new NetVariantVector();
            foreach(var arg in args)
            {
                NetVariant netVariant = new NetVariant();
                Utils.PackValue(arg, netVariant, true);
                result.Add(netVariant);
            }
            return result;
        }
    }
}
