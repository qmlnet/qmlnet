using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qt.NetCore.Internal;

namespace Qt.NetCore.Types
{
    public class NetInstance : BaseDisposable
    {
        private NetInstance(IntPtr gcHandle, NetTypeInfo type)
            :base(Interop.NetInstance.Create(gcHandle, type.Handle))
        {
        }

        public NetInstance(IntPtr handle, bool ownsHandle = true)
            : base(handle, ownsHandle)
        {
            
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
        
        #region Instance helpers
        
        public static ITypeCreator TypeCreator { get; set; }
        
        private static Type GetUnproxiedType(Type type)
        {
            if (type.Namespace == "Castle.Proxies")
                return type.BaseType;

            return type;
        }
        
        private static readonly ConditionalWeakTable<object, NetInstance> ObjectNetInstanceConnections = new ConditionalWeakTable<object, NetInstance>();

        public static NetInstance GetForObject(object value)
        {
            if (value == null) return null;
            var alreadyExists = false;
            if (ObjectNetInstanceConnections.TryGetValue(value, out var netInstance))
            {
                alreadyExists = true;
                if (GCHandle.FromIntPtr(netInstance.Handle).IsAllocated)
                {
                    return netInstance;
                }
            }
            var typeInfo = NetTypeManager.GetTypeInfo(GetUnproxiedType(value.GetType()).AssemblyQualifiedName);
            if(typeInfo == null) throw new InvalidOperationException($"Couldn't create type info from {value.GetType().AssemblyQualifiedName}");
            var handle = GCHandle.Alloc(value);
            var newNetInstance = new NetInstance(GCHandle.ToIntPtr(handle), typeInfo);
            if(alreadyExists)
            {
                ObjectNetInstanceConnections.Remove(value);
            }
            ObjectNetInstanceConnections.Add(value, newNetInstance);
            return newNetInstance;
        }
        
        #endregion
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