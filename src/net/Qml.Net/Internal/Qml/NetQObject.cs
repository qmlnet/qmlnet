using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices;
using Qml.Net.Internal.Types;

namespace Qml.Net.Internal.Qml
{
    internal class NetQObject : BaseDisposable
    {
        public NetQObject(IntPtr handle, bool ownsHandle = true) 
            : base(handle, ownsHandle)
        {
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetQObject.Destroy(ptr);
        }

        public NetVariant GetProperty(string propertyName)
        {
            byte wasSuccessful = 0; 
            var result = Interop.NetQObject.GetProperty(Handle, propertyName, ref wasSuccessful);
            if (wasSuccessful == 0)
            {
                // Dispose of the type before we throw
                using (result == IntPtr.Zero ? null : new NetVariant(result)) { }
                throw new Exception("Getting property failed.");
            }
            return result != IntPtr.Zero ? new NetVariant(result) : null;
        }

        public void SetProperty(string propertyName, NetVariant value)
        {
            byte wasSuccessful = 0; 
            Interop.NetQObject.SetProperty(Handle, propertyName, value?.Handle ?? IntPtr.Zero, ref wasSuccessful);
            if (wasSuccessful == 0)
            {
                throw new Exception("Setting property failed.");
            }
        }

        public NetVariant InvokeMethod(string methodName, NetVariantList parameters)
        {
            byte wasSuccessful = 0;
            var result = Interop.NetQObject.InvokeMethod(Handle, methodName, parameters?.Handle ?? IntPtr.Zero, ref wasSuccessful);
            
            if (wasSuccessful == 0)
            {
                // Dispose of the type before we throw
                using (result == IntPtr.Zero ? null : new NetVariant(result)) { }
                throw new Exception("Invoking method failed.");
            }
            
            return result != IntPtr.Zero ? new NetVariant(result) : null;
        }

        public NetQObjectSignalConnection AttachSignal(string signalName, Del del)
        {
            using (var delReference = NetReference.CreateForObject(del))
            {
                byte wasSuccessful = 0;
                var result = Interop.NetQObject.AttachSignal(Handle, signalName, delReference.Handle, ref wasSuccessful);

                if (wasSuccessful == 0)
                {
                    // Dispose of the type before we throw
                    using (result == IntPtr.Zero ? null : new NetQObjectSignalConnection(result)) { }
                    throw new Exception("Attaching to signal failed.");
                }

                return result == IntPtr.Zero ? null : new NetQObjectSignalConnection(result);
            }
        }
        
        public NetQObjectSignalConnection AttachNotifySignal(string propertyName, Del del)
        {
            using (var delReference = NetReference.CreateForObject(del))
            {
                byte wasSuccessful = 0;
                var result = Interop.NetQObject.AttachNotifySignal(Handle, propertyName, delReference.Handle, ref wasSuccessful);

                if (wasSuccessful == 0)
                {
                    // Dispose of the type before we throw
                    using (result == IntPtr.Zero ? null : new NetQObjectSignalConnection(result)) { }
                    throw new Exception("Attaching to notify signal failed.");
                }

                return result == IntPtr.Zero ? null : new NetQObjectSignalConnection(result);
            }
        }
        
        public dynamic AsDynamic()
        {
            return new NetQObjectDynamic(this);
        }

        internal class NetQObjectDynamic : DynamicObject, INetQObject
        {
            readonly NetQObject _qObject;

            public NetQObjectDynamic(NetQObject qObject)
            {
                _qObject = qObject;
            }

            public void Dispose()
            {
                _qObject.Dispose();
            }

            public NetQObject QObject => _qObject;
            
            public object GetProperty(string propertyName)
            {
                var property = _qObject.GetProperty(propertyName);
                if (property == null)
                {
                    return null;
                }
                
                object unpacked = null;
                Helpers.Unpackvalue(ref unpacked, property);
                property.Dispose();
                return unpacked;
            }

            public void SetProperty(string propertyName, object value)
            {
                if (value == null)
                {
                    _qObject.SetProperty(propertyName, null);
                }
                else
                {
                    using (var variant = new NetVariant())
                    {
                        Helpers.PackValue(value, variant);
                        _qObject.SetProperty(propertyName, variant);
                    }
                }
            }

            public object InvokeMethod(string methodName, params object[] parameters)
            {
                NetVariantList variantParameters = null;
                if (parameters != null && parameters.Length > 0)
                {
                    variantParameters = new NetVariantList();
                    foreach (var parameter in parameters)
                    {
                        using (var variantParameter = NetVariant.From(parameter))
                        {
                            variantParameters.Add(variantParameter);
                        }
                    }
                }
                using (variantParameters)
                using (var result = _qObject.InvokeMethod(methodName, variantParameters))
                {
                    if (result == null)
                    {
                        return null;
                    }
                    object unpacked = null;
                    Helpers.Unpackvalue(ref unpacked, result);
                    result.Dispose();
                    return unpacked;
                }
            }

            public IDisposable AttachSignal(string signalName, SignalHandler handler)
            {
                var del = new Del();
                del.Invoked += parameters =>
                {
                    var result = new List<object>();
                    
                    var parametersCount = parameters.Count;
                    for (var x = 0; x < parametersCount; x++)
                    {
                        using (var parameter = parameters.Get(x))
                        {
                            object parameterValue = null;
                            Helpers.Unpackvalue(ref parameterValue, parameter);
                            result.Add(parameterValue);
                        }
                    }

                    handler(result);
                };
                
                return _qObject.AttachSignal(signalName, del);
            }

            public IDisposable AttachNotifySignal(string propertyName, SignalHandler handler)
            {
                var del = new Del();
                del.Invoked += parameters =>
                {
                    var result = new List<object>();
                    
                    var parametersCount = parameters.Count;
                    for (var x = 0; x < parametersCount; x++)
                    {
                        using (var parameter = parameters.Get(x))
                        {
                            object parameterValue = null;
                            Helpers.Unpackvalue(ref parameterValue, parameter);
                            result.Add(parameterValue);
                        }
                    }

                    handler(result);
                };
                
                return _qObject.AttachNotifySignal(propertyName, del);
            }
            
            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                result = GetProperty(binder.Name);
                // TODO: Check if this was actually a property
                return true;
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                SetProperty(binder.Name, value);
                // TODO: Check if this was actually a property
                return true;
            }
            
            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                result = InvokeMethod(binder.Name, args);
                // TODO: Check if this was actually a method.
                return true;
            }
        }
    }
    
    internal class NetQObjectInterop
    {
        [NativeSymbol(Entrypoint = "net_qobject_destroy")]
        public DestroyDel Destroy { get; set; }

        public delegate void DestroyDel(IntPtr qObject);
        
        [NativeSymbol(Entrypoint = "net_qobject_getProperty")]
        public GetPropertyDel GetProperty { get; set; }

        public delegate IntPtr GetPropertyDel(IntPtr qObject, [MarshalAs(UnmanagedType.LPWStr)] string propertyName, ref byte result);

        [NativeSymbol(Entrypoint = "net_qobject_setProperty")]
        public SetPropertyDel SetProperty { get; set; }
        
        public delegate IntPtr SetPropertyDel(IntPtr qObject, [MarshalAs(UnmanagedType.LPWStr)] string propertyName, IntPtr netVariant, ref byte result);

        [NativeSymbol(Entrypoint = "net_qobject_invokeMethod")]
        public InvokeMethodDel InvokeMethod { get; set; }
        
        public delegate IntPtr InvokeMethodDel(IntPtr qObject, [MarshalAs(UnmanagedType.LPWStr)] string methodName, IntPtr parameters, ref byte result);
        
        [NativeSymbol(Entrypoint = "net_qobject_attachSignal")]
        public AttachSignalDel AttachSignal { get; set; }
        
        public delegate IntPtr AttachSignalDel(IntPtr qObject, [MarshalAs(UnmanagedType.LPWStr)] string signalName, IntPtr del, ref byte result);
        
        [NativeSymbol(Entrypoint = "net_qobject_attachNotifySignal")]
        public AttachNotifySignalDel AttachNotifySignal { get; set; }
        
        public delegate IntPtr AttachNotifySignalDel(IntPtr qObject, [MarshalAs(UnmanagedType.LPWStr)] string signalName, IntPtr del, ref byte result);
    }
}