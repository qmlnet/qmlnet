using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;

namespace Qml.Net.Internal.Qml
{
    internal class NetJsValue : BaseDisposable, INetJsValue
    {
        public NetJsValue(IntPtr handle, bool ownsHandle = true)
            : base(handle, ownsHandle)
        {
            
        }

        public bool IsCallable => Interop.NetJsValue.IsCallable(Handle);
        
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
        
        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetVariant.Destroy(ptr);
        }
    }

    public interface INetJsValue
    {
        bool IsCallable { get; }

        object Call(params object[] parameters);
    }
    
    internal interface INetJsValueInterop
    {
        [NativeSymbol(Entrypoint = "net_js_value_destroy")]
        void Destroy(IntPtr jsValue);

        [NativeSymbol(Entrypoint = "net_js_value_isCallable")]
        bool IsCallable(IntPtr jsValue);
        [NativeSymbol(Entrypoint = "net_js_value_call")]
        IntPtr Call(IntPtr jsValue, IntPtr parameters);
    }
}