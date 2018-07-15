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
            var type = Type.GetType(typeInfo.FullTypeName);
            if(type == null) throw new InvalidOperationException();
            
            typeInfo.ClassName = type.Name;
            Console.WriteLine(typeInfo.ClassName);
        }
    }
}