using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;

namespace Qml.Net.Internal.Qml
{
    public class NetJsValue : BaseDisposable
    {
        public NetJsValue(IntPtr handle, bool ownsHandle = true)
            : base(handle, ownsHandle)
        {
            
        }

        public bool IsCallable => Interop.NetJsValue.IsCallable(Handle);
        
        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetVariant.Destroy(ptr);
        }
    }
    
    internal interface INetJsValueInterop
    {
        [NativeSymbol(Entrypoint = "net_js_value_destroy")]
        void Destroy(IntPtr jsValue);

        [NativeSymbol(Entrypoint = "net_js_value_isCallable")]
        bool IsCallable(IntPtr jsValue);
    }
}