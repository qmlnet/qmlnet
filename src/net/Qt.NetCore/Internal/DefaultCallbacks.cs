using System;

namespace Qt.NetCore.Internal
{
    public class DefaultCallbacks : ICallbacks
    {
        public bool IsTypeValid(string typeName)
        {
            var t = Type.GetType(typeName);
            return t != null;
        }

        public void BuildTypeInfo(NetTypeInfo typeInfo)
        {
            // TODO:
        }
    }
}