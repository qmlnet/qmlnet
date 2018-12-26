using System;

namespace Qml.Net.Internal.Qml
{
    internal class NetVariantList : BaseDisposable
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

    internal class NetVariantListInterop
    {
        [NativeSymbol(Entrypoint = "net_variant_list_create")]
        public CreateDel Create { get; set; }

        public delegate IntPtr CreateDel();

        [NativeSymbol(Entrypoint = "net_variant_list_destroy")]
        public DestroyDel Destroy { get; set; }

        public delegate void DestroyDel(IntPtr list);

        [NativeSymbol(Entrypoint = "net_variant_list_count")]
        public CountDel Count { get; set; }

        public delegate int CountDel(IntPtr list);

        [NativeSymbol(Entrypoint = "net_variant_list_add")]
        public AddDel Add { get; set; }

        public delegate void AddDel(IntPtr list, IntPtr variant);

        [NativeSymbol(Entrypoint = "net_variant_list_get")]
        public GetDel Get { get; set; }

        public delegate IntPtr GetDel(IntPtr list, int index);

        [NativeSymbol(Entrypoint = "net_variant_list_remove")]
        public RemoveDel Remove { get; set; }

        public delegate void RemoveDel(IntPtr list, int index);

        [NativeSymbol(Entrypoint = "net_variant_list_clear")]
        public ClearDel Clear { get; set; }

        public delegate void ClearDel(IntPtr list);
    }
}