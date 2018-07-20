using System;
using System.Collections.Generic;
using System.Linq;
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

        public void BuildTypeInfo(IntPtr t)
        {
            using (var type = new NetTypeInfo(t))
            {
                var typeInfo = Type.GetType(type.FullTypeName);
                if (typeInfo == null) throw new InvalidOperationException();

                type.ClassName = typeInfo.Name;

                if (typeInfo == typeof(bool))
                    type.PrefVariantType = NetVariantType.Bool;
                else if (typeInfo == typeof(char))
                    type.PrefVariantType = NetVariantType.Char;
                else if (typeInfo == typeof(int))
                    type.PrefVariantType = NetVariantType.Int;
                else if (typeInfo == typeof(uint))
                    type.PrefVariantType = NetVariantType.UInt;
                else if (typeInfo == typeof(double))
                    type.PrefVariantType = NetVariantType.Double;
                else if (typeInfo == typeof(string))
                    type.PrefVariantType = NetVariantType.String;
                else if (typeInfo == typeof(DateTime))
                    type.PrefVariantType = NetVariantType.DateTime;
                else
                    type.PrefVariantType = NetVariantType.Object;

                // Don't grab properties and methods for system-level types.
                if (Helpers.IsPrimitive(typeInfo)) return;

                foreach (var methodInfo in typeInfo.GetMethods(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (Helpers.IsPrimitive(methodInfo.DeclaringType)) continue;

                    NetTypeInfo returnType = null;

                    if (methodInfo.ReturnParameter != null && methodInfo.ReturnParameter.ParameterType != typeof(void))
                    {
                        returnType =
                            NetTypeManager.GetTypeInfo(methodInfo.ReturnParameter.ParameterType.AssemblyQualifiedName);
                    }

                    var method = new NetMethodInfo(type, methodInfo.Name, returnType);

                    foreach (var parameter in methodInfo.GetParameters())
                    {
                        method.AddParameter(parameter.Name,
                            NetTypeManager.GetTypeInfo(parameter.ParameterType.AssemblyQualifiedName));
                    }

                    type.AddMethod(method);
                }

                foreach (var propertyInfo in typeInfo.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    if (Helpers.IsPrimitive(propertyInfo.DeclaringType)) continue;

                    using (var property = new NetPropertyInfo(
                        type,
                        propertyInfo.Name,
                        NetTypeManager.GetTypeInfo(propertyInfo.PropertyType.AssemblyQualifiedName),
                        propertyInfo.CanRead,
                        propertyInfo.CanWrite))
                    {
                        type.AddProperty(property);
                    }
                }

                foreach (var signalAttribute in typeInfo.GetCustomAttributes().OfType<SignalAttribute>())
                {
                    using (var signal = new NetSignalInfo(type, signalAttribute.Name))
                    {
                        foreach (var parameter in signalAttribute.Parameters)
                        {
                            signal.AddParameter(parameter);
                        }
                        type.AddSignal(signal);
                    }
                }
            }
        }

        public void ReleaseGCHandle(IntPtr handle)
        {
            ((GCHandle)handle).Free();
        }
        
        public IntPtr InstantiateType(IntPtr type)
        {
            try
            {
                var typeName = Interop.NetTypeInfo.GetFullTypeName(type);
                var typeInfo = Type.GetType(typeName);
                if(typeInfo == null) throw new InvalidOperationException($"Invalid type {typeName}");
            
                var typeCreator = NetInstance.TypeCreator;
                var instance = typeCreator != null ? typeCreator.Create(typeInfo) : Activator.CreateInstance(typeInfo);

                var netInstance = NetInstance.GetForObject(instance);
                // When .NET collects this NetInstance, we don't want it to delete this
                // handle. Ownership has been passed to the caller.
                return Interop.NetInstance.Clone(netInstance.Handle);
            }
            finally
            {
                Interop.NetTypeInfo.Destroy(type);
            }
        }

        public void ReadProperty(IntPtr p, IntPtr t, IntPtr r)
        {
            using(var property = new NetPropertyInfo(p))
            using(var target = new NetInstance(t))
            using(var result = new NetVariant(r))
            {
                var o = target.Instance;

                var propertInfo = o.GetType()
                    .GetProperty(property.Name, BindingFlags.Instance | BindingFlags.Public);
            
                if(propertInfo == null)
                    throw new InvalidOperationException($"Invalid property {property.Name}");

                Helpers.PackValue(propertInfo.GetValue(o), result);
            }
        }

        public void WriteProperty(IntPtr p, IntPtr t, IntPtr v)
        {
            using (var property = new NetPropertyInfo(p))
            using (var target = new NetInstance(t))
            using (var value = new NetVariant(v))
            {
                var o = target.Instance;

                var propertInfo = o.GetType()
                    .GetProperty(property.Name, BindingFlags.Instance | BindingFlags.Public);

                if (propertInfo == null)
                    throw new InvalidOperationException($"Invalid property {property.Name}");

                object newValue = null;
                Helpers.Unpackvalue(ref newValue, value);

                propertInfo.SetValue(o, newValue);
            }
        }

        public void InvokeMethod(IntPtr m, IntPtr t, IntPtr p, IntPtr r)
        {
            using (var method = new NetMethodInfo(m))
            using (var target = new NetInstance(t))
            using (var parameters = new NetVariantList(p))
            using (var result = r != IntPtr.Zero ? new NetVariant(r) : null)
            {
                var instance = target.Instance;
            
                List<object> methodParameters = null;

                if (parameters.Count > 0)
                {
                    methodParameters = new List<object>();
                    var parameterCount = parameters.Count;
                    for (var x = 0; x < parameterCount; x++)
                    {
                        object v = null;
                        Helpers.Unpackvalue(ref v, parameters.Get(x));
                        methodParameters.Add(v);
                    }
                }

                var methodInfo = instance.GetType()
                    .GetMethod(method.MethodName, BindingFlags.Instance | BindingFlags.Public);

                if (methodInfo == null)
                {
                    throw new InvalidOperationException($"Invalid method name {method.MethodName}");
                }

                var returnObject = methodInfo.Invoke(instance, methodParameters?.ToArray());
                
                if (result == null)
                {
                    // this method doesn't have return type
                }
                else
                {
                    Helpers.PackValue(returnObject, result);
                }
            }
        }
    }
}