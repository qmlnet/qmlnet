using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qt.NetCore.Internal;

namespace Qt.NetCore.Qml
{
    public class NetVariant : BaseDisposable
    {
        public NetVariant()
            :base(Interop.NetVariant.Create())
        {
            
        }

        public NetVariantType VariantType => Interop.NetVariant.GetVariantType(Handle);

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetVariant.Destroy(ptr);
        }
    }
    
    public interface INetVariantInterop
    {
        [NativeSymbol(Entrypoint = "net_variant_create")]
        IntPtr Create();
        [NativeSymbol(Entrypoint = "net_variant_destroy")]
        void Destroy(IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_setNetInstance")]
        void SetNetInstance(IntPtr variant, IntPtr instance);
        [NativeSymbol(Entrypoint = "net_variant_getNetInstance")]
        IntPtr GetNetInstance(IntPtr variant);
        
        [NativeSymbol(Entrypoint = "net_variant_getVariantType")]
        NetVariantType GetVariantType(IntPtr variant);
    }
}