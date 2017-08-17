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
                var o = Activator.CreateInstance(Type.GetType(typeInfo.GetTypeName()));
                var handle = GCHandle.Alloc(o);
                instance = GCHandle.ToIntPtr(handle);
            }
            

            public override void ReadProperty(NetPropertyInfo propertyInfo, NetInstance target, NetInstance result)
            {
                //var handleRef = SWIGTYPE_p_void.getCPtr(target.GetValue());
                //var handle = (GCHandle)handleRef.Handle;
                //var o = handle.Target;

                //var value = o.GetType()
                //    .GetProperty(propertyInfo.GetPropertyName(), BindingFlags.Instance | BindingFlags.Public)
                //    .GetValue(o);

                //switch (result.GetInterType())
                //{
                //    case NetInterTypeEnum.NetInterTypeEnum_Bool:
                //        result.SetBool((bool)value);
                //        break;
                //    case NetInterTypeEnum.NetInterTypeEnum_Int:
                //        result.SetInt((int)value);
                //        break;
                //    case NetInterTypeEnum.NetInterTypeEnum_Double:
                //    case NetInterTypeEnum.NetInterTypeEnum_Float:
                //    case NetInterTypeEnum.NetInterTypeEnum_String:
                //    case NetInterTypeEnum.NetInterTypeEnum_Date:
                //    case NetInterTypeEnum.NetInterTypeEnum_Object:
                //        throw new Exception("Unsupported");
                //    default:
                //        throw new Exception("Unsupported");
                //}
            }

            public override void WriteProperty(NetPropertyInfo propertyInfo, NetInstance target, NetInstance value)
            {
                //var handleRef = SWIGTYPE_p_void.getCPtr(target.GetValue());
                //var handle = (GCHandle)handleRef.Handle;
                //var o = handle.Target;

                //var pInfo = o.GetType()
                //    .GetProperty(propertyInfo.GetPropertyName(), BindingFlags.Instance | BindingFlags.Public);

                //switch (value.GetInterType())
                //{
                //    case NetInterTypeEnum.NetInterTypeEnum_Bool:
                //        pInfo.SetValue(o, value.GetBool());
                //        break;
                //    case NetInterTypeEnum.NetInterTypeEnum_Int:
                //        pInfo.SetValue(0, value.GetInt());
                //        break;
                //    case NetInterTypeEnum.NetInterTypeEnum_Double:
                //    case NetInterTypeEnum.NetInterTypeEnum_Float:
                //    case NetInterTypeEnum.NetInterTypeEnum_String:
                //    case NetInterTypeEnum.NetInterTypeEnum_Date:
                //    case NetInterTypeEnum.NetInterTypeEnum_Object:
                //        throw new Exception("Unsupported");
                //    default:
                //        throw new Exception("Unsupported");
                //}
            }

            public override void InvokeMethod(NetMethodInfo @methodInfo, NetInstance @target, NetInstanceVector parameters, NetInstance @result)
            {
                //var handleRef = SWIGTYPE_p_void.getCPtr(target.GetValue());
                //var handle = (GCHandle)handleRef.Handle;
                //var o = handle.Target;

                //List<object> methodParameters = null;

                //if (parameters.Count > 0)
                //{
                //    methodParameters = new List<object>();
                //    foreach (var parameterInstance in parameters)
                //    {
                //        switch (parameterInstance.GetInterType())
                //        {
                //            case NetInterTypeEnum.NetInterTypeEnum_Bool:
                //                methodParameters.Add(parameterInstance.GetBool());
                //                break;
                //            case NetInterTypeEnum.NetInterTypeEnum_Int:
                //                methodParameters.Add(parameterInstance.GetInt());
                //                break;
                //            case NetInterTypeEnum.NetInterTypeEnum_Double:
                //            case NetInterTypeEnum.NetInterTypeEnum_Float:
                //            case NetInterTypeEnum.NetInterTypeEnum_String:
                //            case NetInterTypeEnum.NetInterTypeEnum_Date:
                //            case NetInterTypeEnum.NetInterTypeEnum_Object:
                //                throw new Exception("Unsupported");
                //            default:
                //                throw new Exception("Unsupported");
                //        }
                //    }
                //}

                //var r = o.GetType()
                //    .GetMethod(methodInfo.GetMethodName(), BindingFlags.Instance | BindingFlags.Public)
                //    .Invoke(o, methodParameters?.ToArray());

                //if (r != null)
                //{
                //    var rType = r.GetType();
                //    if (rType == typeof(bool))
                //    {
                //        result.SetBool((bool)r);
                //    }
                //    else if (rType == typeof(int))
                //    {
                //        result.SetInt((int)r);
                //    }
                //    else
                //    {
                //        var gcHandle = GCHandle.Alloc(o);
                //        result.SetValue(new SWIGTYPE_p_void(GCHandle.ToIntPtr(handle), true));
                //    }
                //}
            }

            public override void ReleaseGCHandle(IntPtr gcHandle)
            {
                var handle = (GCHandle)gcHandle;
                handle.Free();
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
