using System;
using System.Collections.Generic;
using System.Linq;
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
                Console.WriteLine("Building info...");
                Console.WriteLine(typeInfo.GetTypeName());
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
