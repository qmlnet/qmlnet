using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Remoting.Messaging;
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
    }
}
