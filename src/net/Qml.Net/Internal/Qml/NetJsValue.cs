using System;
using System.Dynamic;
using System.Runtime.InteropServices;

namespace Qml.Net.Internal.Qml
{
    internal class NetJsValue : BaseDisposable
    {
        public NetJsValue(IntPtr handle, bool ownsHandle = true)
            : base(handle, ownsHandle)
        {
        }

        public bool IsCallable => Interop.NetJsValue.IsCallable(Handle) == 1;

        public bool IsArray => Interop.NetJsValue.IsArray(Handle) == 1;

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

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetJsValue.Destroy(ptr);
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

            public object GetProperty(string propertyName)
            {
                var result = _jsValue.GetProperty(propertyName);
                if (result == null)
                {
                    return null;
                }
                object unpacked = null;
                Helpers.Unpackvalue(ref unpacked, result);
                return unpacked;
            }

            public void SetProperty(string propertyName, object value)
            {
                if (value == null)
                {
                    _jsValue.SetProperty(propertyName, null);
                }
                else
                {
                    using (var variant = new NetVariant())
                    {
                        Helpers.Pack(value, variant, value.GetType());
                        _jsValue.SetProperty(propertyName, variant);
                    }
                }
            }
            
            public object GetItemAtIndex(int arrayIndex)
            {
                var result = _jsValue.GetItemAtIndex(arrayIndex);
                if (result == null)
                {
                    return null;
                }
                object unpacked = null;
                Helpers.Unpackvalue(ref unpacked, result);
                return unpacked;
            }

            public object Call(params object[] parameters)
            {
                return _jsValue.Call(parameters);
            }

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
                SetProperty(binder.Name, value);
                return true;
            }
        }
    }

    internal class NetJsValueInterop
    {
        [NativeSymbol(Entrypoint = "net_js_value_destroy")]
        public DestroyDel Destroy { get; set; }

        public delegate void DestroyDel(IntPtr jsValue);

        [NativeSymbol(Entrypoint = "net_js_value_isCallable")]
        public IsCallableDel IsCallable { get; set; }

        public delegate byte IsCallableDel(IntPtr jsValue);

        [NativeSymbol(Entrypoint = "net_js_value_isArray")]
        public IsArrayDel IsArray { get; set; }

        public delegate byte IsArrayDel(IntPtr jsValue);

        [NativeSymbol(Entrypoint = "net_js_value_call")]
        public CallDel Call { get; set; }

        public delegate IntPtr CallDel(IntPtr jsValue, IntPtr parameters);

        [NativeSymbol(Entrypoint = "net_js_value_getProperty")]
        public GetPropertyDel GetProperty { get; set; }

        public delegate IntPtr GetPropertyDel(IntPtr jsValue, [MarshalAs(UnmanagedType.LPWStr)] string propertyName);

        [NativeSymbol(Entrypoint = "net_js_value_getItemAtIndex")]
        public GetItemAtIndexDel GetItemAtIndex { get; set; }

        public delegate IntPtr GetItemAtIndexDel(IntPtr jsValue, int arrayIndex);

        [NativeSymbol(Entrypoint = "net_js_value_setProperty")]
        public SetPropertyDel SetProperty { get; set; }

        public delegate void SetPropertyDel(IntPtr jsValue, [MarshalAs(UnmanagedType.LPWStr)] string propertyName, IntPtr value);
    }
}