using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using Qt.NetCore.Qml;
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

        public void ReleaseGCHandle(IntPtr handle)
        {
            ((GCHandle)handle).Free();
        }
        
        public GCHandle InstantiateType(string typeName)
        {
            var type = Type.GetType(typeName);
            if(type == null) throw new InvalidOperationException($"Invalid type {typeName}");
            
            var typeCreator = NetInstance.TypeCreator;
            object instance = typeCreator != null ? typeCreator.Create(type) : Activator.CreateInstance(type);
            
            var instanceHandle = GCHandle.Alloc(instance);
            return instanceHandle;
        }

        public void ReadProperty(NetPropertyInfo property, NetInstance target, NetVariant result)
        {
            var o = target.Instance;

            var propertInfo = o.GetType()
                .GetProperty(property.Name, BindingFlags.Instance | BindingFlags.Public);
            
            if(propertInfo == null)
                throw new InvalidOperationException($"Invalid property {property.Name}");

            var value = propertInfo.GetValue(o);
            
            PackValue(ref value, result);
        }

        public void WriteProperty(NetPropertyInfo property, NetInstance target, NetVariant value)
        {
            var o = target.Instance;

            var propertInfo = o.GetType()
                .GetProperty(property.Name, BindingFlags.Instance | BindingFlags.Public);

            if(propertInfo == null)
                throw new InvalidOperationException($"Invalid property {property.Name}");
            
            object newValue = null;
            Unpackvalue(ref newValue, value);

            propertInfo.SetValue(o, newValue);
        }

        public void InvokeMethod(NetMethodInfo method, NetInstance target, NetVariantList parameters, NetVariant result)
        {
            var instance = target.Instance;
            
            List<object> methodParameters = null;

            if (parameters.Count > 0)
            {
                methodParameters = new List<object>();
                int parameterCount = parameters.Count;
                for (var x = 0; x < parameterCount; x++)
                {
                    object v = null;
                    Unpackvalue(ref v, parameters.Get(x));
                    methodParameters.Add(v);
                }
            }

            var methodInfo = instance.GetType()
                .GetMethod(method.MethodName, BindingFlags.Instance | BindingFlags.Public);

            if (methodInfo == null)
            {
                throw new InvalidOperationException($"Invalid method name {method.MethodName}");
            }
            
            var r = methodInfo.Invoke(instance, methodParameters?.ToArray());

            if (result == null)
            {
                // this method doesn't have return type
            }
            else
            {
                PackValue(ref r, result);
            }
        }

        private bool IsPrimitive(Type type)
        {
            if (type.Namespace == "System") return true;
            return false;
        }
        
        private void PackValue(ref object source, NetVariant destination)
        {
            if (source == null)
            {
                destination.Clear();
            }
            else
            {
                var type = source.GetType();
                if (type == typeof(bool))
                    destination.Bool = (bool)source;
                else if(type == typeof(char))
                    destination.Char = (char)source;
                else if(type == typeof(double))
                    destination.Double = (double)source;
                else if (type == typeof(int))
                    destination.Int = (int)source;
                else if(type == typeof(uint))
                    destination.UInt = (uint)source;
                else if (type == typeof(string))
                    destination.String = (string)source;
                else if(type == typeof(DateTime))
                    destination.DateTime = (DateTime)source;
                else
                {
                    destination.Instance = NetInstance.CreateFromObject(source);
                }
            }
        }

        private void Unpackvalue(ref object destination, NetVariant source)
        {
            switch (source.VariantType)
            {
                case NetVariantType.Invalid:
                    destination = null;
                    break;
                case NetVariantType.Bool:
                    destination = source.Bool;
                    break;
                case NetVariantType.Char:
                    destination = source.Char;
                    break;
                case NetVariantType.Int:
                    destination = source.Int;
                    break;
                case NetVariantType.UInt:
                    destination = source.UInt;
                    break;
                case NetVariantType.Double:
                    destination = source.Double;
                    break;
                case NetVariantType.String:
                    destination = source.String;
                    break;
                case NetVariantType.DateTime:
                    destination = source.DateTime;
                    break;
                case NetVariantType.Object:
                    destination = source.Instance.Instance;
                    break;
                default:
                    throw new Exception("Unsupported variant type.");
            }
        }
    }
}