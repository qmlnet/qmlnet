using System;
using AdvancedDLSupport;
using Qt.NetCore.Internal;

namespace Qt.NetCore.Qml
{
    public class NetVariantList : BaseDisposable
    {
        public NetVariantList()
            : this(Interop.NetVariantList.Create())
        {
            
        }

        public NetVariantList(IntPtr handle, bool ownsHandle = true)
            : base(handle, ownsHandle)
        {
            
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetVariantList.Destroy(ptr);
        }
    }
    
    public interface INetVariantListInterop
    {   
        [NativeSymbol(Entrypoint = "net_variant_list_create")]
        IntPtr Create();
        [NativeSymbol(Entrypoint = "net_variant_list_destroy")]
        void Destroy(IntPtr variant);
    }
}