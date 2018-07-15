using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qt.NetCore.Internal;

namespace Qt.NetCore.Types
{
    public class NetInstance : BaseDisposable
    {
        public NetInstance(IntPtr gcHandle, NetTypeInfo type)
            :base(Interop.NetInstance.Create(gcHandle, type.Handle))
        {
        }

        public NetInstance(IntPtr handle, bool ownsHandle = true)
            : base(handle, ownsHandle)
        {
            
        }

        public static NetInstance CreateFromObject(object value)
        {
            if (value == null) return null;
            var typeInfo = NetTypeManager.GetTypeInfo(value.GetType().AssemblyQualifiedName);
            if(typeInfo == null) throw new InvalidOperationException($"Couldn't create type info from {value.GetType().AssemblyQualifiedName}");
            var handle = GCHandle.Alloc(value);
            return new NetInstance(GCHandle.ToIntPtr(handle), typeInfo);
        }

        public object Instance
        {
            get
            {
                var handle = Interop.NetInstance.GetHandle(Handle);
                return ((GCHandle) handle).Target;
            }
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetInstance.Destroy(ptr);
        }
    }

    public interface INetInstanceInterop
    {   
        [NativeSymbol(Entrypoint = "net_instance_create")]
        IntPtr Create(IntPtr handle, IntPtr type);
        [NativeSymbol(Entrypoint = "net_instance_destroy")]
        void Destroy(IntPtr instance);

        [NativeSymbol(Entrypoint = "net_instance_getHandle")]
        IntPtr GetHandle(IntPtr instance);
    }
}