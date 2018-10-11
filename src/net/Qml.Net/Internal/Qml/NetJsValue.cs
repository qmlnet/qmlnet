using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.InteropServices;
using AdvancedDLSupport;

namespace Qml.Net.Internal.Qml
{
    internal class NetJsValue : BaseDisposable
    {
        public NetJsValue(IntPtr handle, bool ownsHandle = true)
            : base(handle, ownsHandle)
        {
            
        }

        public bool IsCallable => Interop.NetJsValue.IsCallable(Handle);

        public bool IsArray => Interop.NetJsValue.IsArray(Handle);

        public bool IsDouble => Interop.NetJsValue.IsDouble(Handle);

        public bool IsString => Interop.NetJsValue.IsString(Handle);
        
        public NetVariant Call(NetVariantList parameters)
        {
            var result = Interop.NetJsValue.Call(Handle, parameters?.Handle ?? IntPtr.Zero);
            return result != IntPtr.Zero ? new NetVariant(result) : null;
        }
        
        public object Call(params object[] parameters)
        {
            NetVariantList variants = null;
            
            if (parameters != null && parameters.Length > 0)
            {
                variants = new NetVariantList();
                foreach (var parameter in parameters)
                {
                    using (var variant = new NetVariant())
                    {
                        Helpers.PackValue(parameter, variant);
                        variants.Add(variant);
                    }
                }
            }
            
            var result = Call(variants);
            
            variants?.Dispose();

            if (result == null)
            {
                return null;
            }
            
            object returnValue = null;
            Helpers.Unpackvalue(ref returnValue, result);
            result.Dispose();
            
            return returnValue;
        }

        public NetVariant GetProperty(string propertyName)
        {
            var result = Interop.NetJsValue.GetProperty(Handle, propertyName);
            if (result == IntPtr.Zero)
            {
                return null;
            }
            return new NetVariant(result);
        }

        public NetVariant GetItemAtIndex(int arrayIndex)
        {
            var result = Interop.NetJsValue.GetItemAtIndex(Handle, arrayIndex);
            if (result == IntPtr.Zero)
            {
                return null;
            }
            return new NetVariant(result);
        }

        public void SetProperty(string propertyName, NetVariant value)
        {
            Interop.NetJsValue.SetProperty(Handle, propertyName, value?.Handle ?? IntPtr.Zero);
        }

        public double ToDouble()
        {
            return Interop.NetJsValue.ToDouble(Handle);
        }
        
        public new string ToString()
        {
            return Interop.NetJsValue.ToString(Handle);
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetVariant.Destroy(ptr);
        }

        public dynamic AsDynamic()
        {
            return new NetJsValueDynamic(this);
        }

        internal class NetJsValueDynamic : DynamicObject, INetJsValue
        {
            readonly NetJsValue _jsValue;

            public NetJsValueDynamic(NetJsValue jsValue)
            {
                _jsValue = jsValue;
            }

            public void Dispose()
            {
                _jsValue.Dispose();
            }

            public NetJsValue JsValue => _jsValue;
            
            public bool IsCallable => _jsValue.IsCallable;

            public bool IsArray => _jsValue.IsArray;

            public bool IsDouble => _jsValue.IsDouble;

            public bool IsString => _jsValue.IsString;

            public object GetProperty(string propertyName) => _jsValue.GetProperty(propertyName);
            
            public object GetItemAtIndex(int arrayIndex) => _jsValue.GetItemAtIndex(arrayIndex);

            public object Call(params object[] parameters)
            {
                return _jsValue.Call(parameters);
            }

            public double ToDouble() =>_jsValue.ToDouble();

            public new string ToString() => _jsValue.ToString();

            public override bool TryInvoke(InvokeBinder binder, object[] args, out object result)
            {
                result = null;
                
                if (!IsCallable)
                {
                    return false;
                }

                result = Call(args);
                
                return true;
            }

            public override bool TryGetMember(GetMemberBinder binder, out object result)
            {
                var property = _jsValue.GetProperty(binder.Name);
                if (property == null)
                {
                    result = null;
                }
                else
                {
                    object unpacked = null;
                    Helpers.Unpackvalue(ref unpacked, property);
                    result = unpacked;
                }
                return true;
            }

            public override bool TrySetMember(SetMemberBinder binder, object value)
            {
                if (value == null)
                {
                    _jsValue.SetProperty(binder.Name, null);
                }
                else
                {
                    using (var variant = new NetVariant())
                    {
                        Helpers.PackValue(value, variant);
                        _jsValue.SetProperty(binder.Name, variant);
                    }
                }

                return true;
            }
        }
    }

    internal interface INetJsValueInterop
    {
        [NativeSymbol(Entrypoint = "net_js_value_destroy")]
        void Destroy(IntPtr jsValue);

        [NativeSymbol(Entrypoint = "net_js_value_isCallable")]
        bool IsCallable(IntPtr jsValue);
        [NativeSymbol(Entrypoint = "net_js_value_isArray")]
        bool IsArray(IntPtr jsValue);
        [NativeSymbol(Entrypoint = "net_js_value_isNumber")]
        bool IsDouble(IntPtr jsValue);
        [NativeSymbol(Entrypoint = "net_js_value_isString")]
        bool IsString(IntPtr jsValue);
        [NativeSymbol(Entrypoint = "net_js_value_call")]
        IntPtr Call(IntPtr jsValue, IntPtr parameters);
        [NativeSymbol(Entrypoint = "net_js_value_getProperty")]
        IntPtr GetProperty(IntPtr jsValue, [MarshalAs(UnmanagedType.LPWStr), CallerFree] string propertyName);
        [NativeSymbol(Entrypoint = "net_js_value_getItemAtIndex")]
        IntPtr GetItemAtIndex(IntPtr jsValue, int arrayIndex);
        [NativeSymbol(Entrypoint = "net_js_value_setProperty")]
        void SetProperty(IntPtr jsValue, [MarshalAs(UnmanagedType.LPWStr), CallerFree] string propertyName, IntPtr value);
        [NativeSymbol(Entrypoint = "net_js_value_toNumber")]
        double ToDouble(IntPtr jsValue);
        [NativeSymbol(Entrypoint = "net_js_value_toString")]
        string ToString(IntPtr jsValue);
    }
}