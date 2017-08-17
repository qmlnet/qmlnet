using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Remoting;
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
                    if (method.DeclaringType == typeof(Object)) continue;

                    NetTypeInfo returnType = null;

                    if (method.ReturnParameter.ParameterType != typeof(void))
                    {
                        returnType = NetTypeInfoManager.GetTypeInfo(
                                method.ReturnParameter.ParameterType.FullName + ", " +
                                method.ReturnParameter.ParameterType.Assembly.FullName);
                    }

                    var methodInfo = NetTypeInfoManager.NewMethodInfo(typeInfo, method.Name, returnType);

                    foreach (var parameter in method.GetParameters())
                    {
                        methodInfo.AddParameter(parameter.Name, NetTypeInfoManager.GetTypeInfo(parameter.ParameterType.FullName + ", " + parameter.ParameterType.Assembly.FullName));
                    }

                    typeInfo.AddMethod(methodInfo);
                }

                foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                {
                    typeInfo.AddProperty(NetTypeInfoManager.NewPropertyInfo(
                        typeInfo, property.Name,
                        NetTypeInfoManager.GetTypeInfo(property.PropertyType.FullName + ", " + property.PropertyType.Assembly.FullName),
                        property.CanRead,
                        property.CanWrite));
                }
            }

            public override void CreateInstance(NetTypeInfo typeInfo, ref IntPtr instance)
            {
                Console.WriteLine("Create");
                var o = Activator.CreateInstance(Type.GetType(typeInfo.GetTypeName()));
                var handle = GCHandle.Alloc(o);
                instance = GCHandle.ToIntPtr(handle);
            }

            public override void ReadProperty(NetPropertyInfo propertyInfo, NetInstance target, NetVariant result)
            {
                var handle = (GCHandle)target.GetGCHandle();
                var o = handle.Target;

                var value = o.GetType()
                    .GetProperty(propertyInfo.GetPropertyName(), BindingFlags.Instance | BindingFlags.Public)
                    .GetValue(o);
                
                PackValue(ref value, result);
            }

            public override void WriteProperty(NetPropertyInfo propertyInfo, NetInstance target, NetVariant value)
            {
                base.WriteProperty(propertyInfo, target, value);
                
                var handle = (GCHandle)target.GetGCHandle();
                var o = handle.Target;

                var pInfo = o.GetType()
                    .GetProperty(propertyInfo.GetPropertyName(), BindingFlags.Instance | BindingFlags.Public);

                object newValue = null;
                Unpackvalue(ref newValue, value);
                
                pInfo.SetValue(o, newValue);
            }
            

            public override void InvokeMethod(NetMethodInfo methodInfo, NetInstance target, NetVariantVector parameters,
                NetVariant result)
            {
                var handle = (GCHandle)target.GetGCHandle();
                var o = handle.Target;

                List<object> methodParameters = null;

                if (parameters.Count > 0)
                {
                    methodParameters = new List<object>();
                    foreach (var parameterInstance in parameters)
                    {
                        object v = null;
                        Unpackvalue(ref v, parameterInstance);
                        methodParameters.Add(v);
                    }
                }

                var r = o.GetType()
                    .GetMethod(methodInfo.GetMethodName(), BindingFlags.Instance | BindingFlags.Public)
                    .Invoke(o, methodParameters?.ToArray());

                if (result == null)
                {
                    // this method doesn't have return type

                }
                else
                {
                    PackValue(ref r, result);
                }
            }
            
            public override void ReleaseGCHandle(IntPtr gcHandle)
            {
                Console.WriteLine("Release");
                var handle = (GCHandle)gcHandle;
                handle.Free();
            }

            public override void CopyGCHandle(IntPtr gcHandle, ref IntPtr gcHandleCopy)
            {
                Console.WriteLine("Copy");
                var handle = (GCHandle)gcHandle;
                var duplicatedHandle = GCHandle.Alloc(handle.Target);
                gcHandleCopy = GCHandle.ToIntPtr(duplicatedHandle);
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
                        destination.SetBool((bool)source);
                    else if (type == typeof(int))
                        destination.SetInt((int)source);
                    else
                    {
                        destination.SetNetInstance(NetTypeInfoManager.WrapCreatedInstance(
                            GCHandle.ToIntPtr(GCHandle.Alloc(source)),
                            NetTypeInfoManager.GetTypeInfo(type.FullName + ", " + type.Assembly.FullName)));
                    }
                }
            }

            private void Unpackvalue(ref object destination, NetVariant source)
            {
                switch (source.GetVariantType())
                {
                    case NetVariantTypeEnum.NetVariantTypeEnum_Invalid:
                        destination = null;
                        break;
                    case NetVariantTypeEnum.NetVariantTypeEnum_Bool:
                        destination = source.GetBool();
                        break;
                    case NetVariantTypeEnum.NetVariantTypeEnum_Int:
                        destination = source.GetInt();
                        break;
                    case NetVariantTypeEnum.NetVariantTypeEnum_Double:
                    case NetVariantTypeEnum.NetVariantTypeEnum_String:
                    case NetVariantTypeEnum.NetVariantTypeEnum_Date:
                        break;
                    case NetVariantTypeEnum.NetVariantTypeEnum_Object:
                        var netInstance = source.GetNetInstance();
                        var gcHandle = (GCHandle) netInstance.GetGCHandle();
                        destination = gcHandle.Target;
                        break;
                    default:
                        throw new Exception("Unsupported variant type.");
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
