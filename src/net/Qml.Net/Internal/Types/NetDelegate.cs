using System;
using System.Runtime.InteropServices;

namespace Qml.Net.Internal.Types
{
    internal class NetDelegate : BaseDisposable
    {
        public NetDelegate(IntPtr handle)
            : base(handle)
        {
        }

        public static NetDelegate FromDelegate(Delegate del)
        {
            var handle = GCHandle.Alloc(del);
            return new NetDelegate(Interop.NetDelegate.Create(GCHandle.ToIntPtr(handle)));
        }

        internal static void ReleaseGCHandle(GCHandle handle)
        {
            handle.Free();
        }

        public Delegate Delegate
        {
            get
            {
                var handle = (GCHandle)Interop.NetDelegate.GetHandle(Handle);
                return (Delegate)handle.Target;
            }
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetDelegate.Destroy(ptr);
        }
    }

    internal class NetDelegateInterop
    {
        [NativeSymbol(Entrypoint = "delegate_create")]
        public CreateDel Create { get; set; }

        public delegate IntPtr CreateDel(IntPtr handle);

        [NativeSymbol(Entrypoint = "delegate_destroy")]
        public DestroyDel Destroy { get; set; }

        public delegate void DestroyDel(IntPtr del);

        [NativeSymbol(Entrypoint = "delegate_getHandle")]
        public GetHandleDel GetHandle { get; set; }

        public delegate IntPtr GetHandleDel(IntPtr del);
    }
}