using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qt.NetCore.Internal;

namespace Qt.NetCore.Types
{
    public class NetInstance : BaseDisposable
    {
        static NetInstance()
        {
            
        }
        
        public NetInstance(IntPtr gcHandle, NetTypeInfo type)
            :base(Interop.NetInstance.Create(gcHandle, type.Handle))
        {
        }

        public NetInstance(IntPtr handle, bool ownsHandle = true)
            : base(handle, ownsHandle)
        {
            
        }

        private static ConditionalWeakTable<object, NetInstance> _objectNetInstanceConnections = new ConditionalWeakTable<object, NetInstance>();

        public static NetInstance GetForObject(object value)
        {
            if (value == null) return null;
            bool alreadyExists = false;
            if (_objectNetInstanceConnections.TryGetValue(value, out var netInstance))
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
                _objectNetInstanceConnections.Remove(value);
            }
            _objectNetInstanceConnections.Add(value, newNetInstance);
            return newNetInstance;
        }
        
        public static NetInstance InstantiateType(NetTypeInfo type)
        {
            var result = Interop.Callbacks.InstantiateType(type.Handle);
            if (result == IntPtr.Zero) return null;
            return new NetInstance(result);
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
        
        public static ITypeCreator TypeCreator { get; set; }
        
        private static Type GetUnproxiedType(Type type)
        {
            if (type.Namespace == "Castle.Proxies")
                return type.BaseType;

            return type;
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