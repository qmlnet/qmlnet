using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;

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

    internal interface INetDelegateInterop
    {
        [NativeSymbol(Entrypoint = "delegate_create")]
        IntPtr Create(IntPtr handle);
        [NativeSymbol(Entrypoint = "delegate_destroy")]
        void Destroy(IntPtr del);
        
        [NativeSymbol(Entrypoint = "delegate_getHandle")]
        IntPtr GetHandle(IntPtr del);
    }
}