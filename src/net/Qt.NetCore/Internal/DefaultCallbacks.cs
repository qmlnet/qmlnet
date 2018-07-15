using System;
using System.Reflection;
using Qt.NetCore.Types;

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

            if (type == typeof(bool))
                typeInfo.PrefVariantType = NetVariantType.Bool;
            else if (type == typeof(char))
                typeInfo.PrefVariantType = NetVariantType.Char;
            else if (type == typeof(int))
                typeInfo.PrefVariantType = NetVariantType.Int;
            else if (type == typeof(uint))
                typeInfo.PrefVariantType = NetVariantType.UInt;
            else if (type == typeof(double))
                typeInfo.PrefVariantType = NetVariantType.Double;
            else if (type == typeof(string))
                typeInfo.PrefVariantType = NetVariantType.String;
            else if (type == typeof(DateTime))
                typeInfo.PrefVariantType = NetVariantType.DateTime;
            else
                typeInfo.PrefVariantType = NetVariantType.Object;

            // Don't grab properties and methods for system-level types.
            if (IsPrimitive(type)) return;
            
            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if(IsPrimitive(method.DeclaringType)) continue;
                
                NetTypeInfo returnType = null;

                if (method.ReturnParameter != null && method.ReturnParameter.ParameterType != typeof(void))
                {
                    returnType = NetTypeManager.GetTypeInfo(method.ReturnParameter.ParameterType.AssemblyQualifiedName);
                }

                var methodInfo = new NetMethodInfo(typeInfo, method.Name, returnType);

                foreach (var parameter in method.GetParameters())
                {
                    methodInfo.AddParameter(parameter.Name, NetTypeManager.GetTypeInfo(parameter.ParameterType.AssemblyQualifiedName));
                }

                typeInfo.AddMethod(methodInfo);
            }

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if(IsPrimitive(property.DeclaringType)) continue;
                
                using (var propertyInfo = new NetPropertyInfo(
                    typeInfo,
                    property.Name,
                    NetTypeManager.GetTypeInfo(property.PropertyType.AssemblyQualifiedName),
                    property.CanRead,
                    property.CanWrite))
                {
                    typeInfo.AddProperty(propertyInfo);
                }
            }
        }

        private bool IsPrimitive(Type type)
        {
            if (type == typeof(Object))
                return true;
            if (type.IsPrimitive) 
                return true;
            if (type == typeof(string))
                return true;
            if (type == typeof(int))
                return true;
            return false;
        }
    }
}