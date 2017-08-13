using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Qt.NetCore
{
    public static class Initializer
    {
        static Callback _callback;
        static readonly object Lock = new object();

        internal class Callback : NetTypeInfoCallbacks
        {
            public override bool isValidType(string typeName)
            {
                var type = Type.GetType(typeName);
                return type != null;
            }

            public override void BuildTypeInfo(NetTypeInfo typeInfo)
            {
                var type = Type.GetType(typeInfo.GetTypeName());

                if (type.Namespace == "System")
                    return; // built in type!

                foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    if(method.DeclaringType == typeof(Object)) continue;

                    var methodInfo = NetTypeInfoManager.NewMethodInfo(typeInfo, method.Name);
                    
                    if (method.ReturnParameter.ParameterType != typeof(void))
                    {
                        methodInfo.SetReturnType(
                            NetTypeInfoManager.GetTypeInfo(
                                method.ReturnParameter.ParameterType.FullName + ", " +
                                method.ReturnParameter.ParameterType.Assembly.FullName));
                    }
                    foreach (var parameter in method.GetParameters())
                    {
                        methodInfo.AddParameter(parameter.Name, NetTypeInfoManager.GetTypeInfo(parameter.ParameterType.FullName + ", " + parameter.ParameterType.Assembly.FullName));
                    }
                }
            }
        }

        public static void Initialize()
        {
            lock (Lock)
            {
                if (_callback == null)
                {
                    _callback = new Callback();
                    NetTypeInfoManager.setCallbacks(_callback);
                }
            }
        }
    }
}
