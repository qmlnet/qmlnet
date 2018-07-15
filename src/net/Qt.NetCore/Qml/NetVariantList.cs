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

        public int Count => Interop.NetVariantList.Count(Handle);

        public void Add(NetVariant variant)
        {
            Interop.NetVariantList.Add(Handle, variant.Handle);
        }

        public NetVariant Get(int index)
        {
            var result = Interop.NetVariantList.Get(Handle, index);
            if (result == IntPtr.Zero) return null;
            return new NetVariant(result);
        }

        public void Remove(int index)
        {
            Interop.NetVariantList.Remove(Handle, index);
        }

        public void Clear()
        {
            Interop.NetVariantList.Clear(Handle);
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
        void Destroy(IntPtr list);

        [NativeSymbol(Entrypoint = "net_variant_list_count")]
        int Count(IntPtr list);

        [NativeSymbol(Entrypoint = "net_variant_list_add")]
        void Add(IntPtr list, IntPtr variant);
        [NativeSymbol(Entrypoint = "net_variant_list_get")]
        IntPtr Get(IntPtr list, int index);
        [NativeSymbol(Entrypoint = "net_variant_list_remove")]
        void Remove(IntPtr list, int index);
        [NativeSymbol(Entrypoint = "net_variant_list_clear")]
        void Clear(IntPtr list);
    }
}