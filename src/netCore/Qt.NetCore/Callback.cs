using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;

namespace Qt.NetCore
{
    internal class Callback : NetTypeInfoCallbacks
    {
        public static Callback Instance
        {
            get;
            private set;
        }

        public Callback()
        {
            Instance = this;
        }

        private IUiContext _UiContext = null;

        public void SetUiContext(IUiContext uiContext)
        {
            _UiContext = uiContext;
        }

        public override bool isValidType(string typeName)
        {
            var type = Type.GetType(typeName);
            return type != null;
        }

        public override void BuildTypeInfo(NetTypeInfo typeInfo)
        {
            var type = Type.GetType(typeInfo.GetFullTypeName());
            
            typeInfo.SetClassName(type.Name);

            if(type == typeof(bool))
                typeInfo.SetPrefVariantType(NetVariantTypeEnum.NetVariantTypeEnum_Bool);
            else if(type == typeof(char))
                typeInfo.SetPrefVariantType(NetVariantTypeEnum.NetVariantTypeEnum_Char);
            else if (type == typeof(int))
                typeInfo.SetPrefVariantType(NetVariantTypeEnum.NetVariantTypeEnum_Int);
            else if (type == typeof(uint))
                typeInfo.SetPrefVariantType(NetVariantTypeEnum.NetVariantTypeEnum_UInt);
            else if(type == typeof(double))
                typeInfo.SetPrefVariantType(NetVariantTypeEnum.NetVariantTypeEnum_Double);
            else if(type == typeof(string))
                typeInfo.SetPrefVariantType(NetVariantTypeEnum.NetVariantTypeEnum_String);
            else if (type == typeof(DateTime))
                typeInfo.SetPrefVariantType(NetVariantTypeEnum.NetVariantTypeEnum_DateTime);
            else
                typeInfo.SetPrefVariantType(NetVariantTypeEnum.NetVariantTypeEnum_Object);

            if (type.Namespace == "System")
                return; // built in type!


            foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Instance))
            {
                if (method.DeclaringType == typeof(Object)) continue;

                //ignore system stuff like event methods
                if (method.IsSpecialName) continue;

                NetTypeInfo returnType = null;

                if(typeof(Task).IsAssignableFrom(method.ReturnParameter.ParameterType))
                {
                    //.NET methods that return a Task don't reflect their return value into Qt
                }
                else if (method.ReturnParameter.ParameterType != typeof(void))
                {
                    returnType = NetTypeInfoManager.GetTypeInfo(method.ReturnParameter.ParameterType);
                }

                var methodInfo = NetTypeInfoManager.NewMethodInfo(typeInfo, method.Name, returnType);

                foreach (var parameter in method.GetParameters())
                {
                    if (parameter.Name != null)
                    {
                        methodInfo.AddParameter(parameter.Name, NetTypeInfoManager.GetTypeInfo(parameter.ParameterType));
                    }
                }

                typeInfo.AddMethod(methodInfo);
            }

            bool implementsINotifyPropertyChanged = typeof(INotifyPropertyChanged).IsAssignableFrom(type);

            foreach (var property in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                //ignore system stuff like events
                if (property.IsSpecialName) continue;

                string notifySignalName = "";
                if (implementsINotifyPropertyChanged)
                {
                    notifySignalName = Utils.CalculatePropertyChangedSignalName(property.Name);
                }
                typeInfo.AddProperty(NetTypeInfoManager.NewPropertyInfo(
                    typeInfo, property.Name,
                    NetTypeInfoManager.GetTypeInfo(property.PropertyType),
                    property.CanRead,
                    property.CanWrite,
                    notifySignalName));
            }
        }

        public override void CreateInstance(NetTypeInfo typeInfo, ref IntPtr instance)
        {
            var type = Type.GetType(typeInfo.GetFullTypeName());

            var typeCreator = NetTypeInfoManager.TypeCreator;
            
            object netInstance = typeCreator != null ? typeCreator.Create(type) : Activator.CreateInstance(type);
            var handle = GCHandle.Alloc(netInstance);
            instance = GCHandle.ToIntPtr(handle);

            Utils.TryAttachNotifyPropertyChanged(netInstance, handle);
        }

        public override void ReadProperty(NetPropertyInfo propertyInfo, NetInstance target, NetVariant result)
        {
            var handle = (GCHandle)target.GetGCHandle();
            var o = handle.Target;

            var value = o.GetType()
                .GetProperty(propertyInfo.GetPropertyName(), BindingFlags.Instance | BindingFlags.Public)
                .GetValue(o);

            Utils.PackValue(value, result, true);
        }

        public override void WriteProperty(NetPropertyInfo propertyInfo, NetInstance target, NetVariant value)
        {
            base.WriteProperty(propertyInfo, target, value);

            var handle = (GCHandle)target.GetGCHandle();
            var o = handle.Target;

            var pInfo = o.GetType()
                .GetProperty(propertyInfo.GetPropertyName(), BindingFlags.Instance | BindingFlags.Public);

            object newValue = null;
            Utils.Unpackvalue(ref newValue, value);

            pInfo.SetValue(o, newValue);
        }

        class QtSynchronizationContext : SynchronizationContext
        {
            private IUiContext _UiContext;

            public QtSynchronizationContext(IUiContext uiContext)
            {
                _UiContext = uiContext;
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                if (_UiContext != null)
                {
                    _UiContext.InvokeOnGuiThread(() => d.Invoke(state));
                }
                else
                {
                    base.Post(d, state);
                }
            }
        }

        class QtSynchronizationContextHandler : IDisposable
        {
            private SynchronizationContext _OriginalSynchronizationContext;
            public QtSynchronizationContextHandler(IUiContext uiContext)
            {
                _OriginalSynchronizationContext = SynchronizationContext.Current;
                SynchronizationContext.SetSynchronizationContext(new QtSynchronizationContext(uiContext));
            }

            public void Dispose()
            {
                SynchronizationContext.SetSynchronizationContext(_OriginalSynchronizationContext);
            }
        }

        public override void InvokeMethod(NetMethodInfo methodInfo, NetInstance target, NetVariantVector parameters,
            NetVariant result)
        {
            using (new QtSynchronizationContextHandler(_UiContext))
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
                        Utils.Unpackvalue(ref v, parameterInstance);
                        methodParameters.Add(v);
                    }
                }

                if (string.Equals("ToString", methodInfo.GetMethodName()))
                {
                    var resultValue = o.ToString();
                    Utils.PackValue(resultValue, result, true);
                }
                else
                {
                    var method = o.GetType()
                        .GetMethod(methodInfo.GetMethodName(), BindingFlags.Instance | BindingFlags.Public);
                    if (typeof(Task).IsAssignableFrom(method.ReturnType))
                    {
                        method.Invoke(o, methodParameters?.ToArray());
                        //Task doesn't get returned
                    }
                    else
                    {
                        var resultValue = method.Invoke(o, methodParameters?.ToArray());
                        Utils.PackValue(resultValue, result, true);
                    }
                }
            }
        }

        public override void ReleaseGCHandle(IntPtr gcHandle)
        {
            var handle = GCHandle.FromIntPtr(gcHandle);
            handle.Free();
        }

        public override void CopyGCHandle(IntPtr gcHandle, ref IntPtr gcHandleCopy)
        {
            var handle = GCHandle.FromIntPtr(gcHandle);
            var duplicatedHandle = GCHandle.Alloc(handle.Target);
            gcHandleCopy = GCHandle.ToIntPtr(duplicatedHandle);
        }
    }
}
