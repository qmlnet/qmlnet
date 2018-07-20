using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qt.NetCore.Internal;
using Qt.NetCore.Qml;

namespace Qt.NetCore.Types
{
    public class NetInstance : BaseDisposable
    {
        private NetInstance(IntPtr gcHandle, NetTypeInfo type, bool ownsHandle = true)
            :base(Interop.NetInstance.Create(gcHandle, type.Handle), ownsHandle)
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

        public NetInstance Clone()
        {
            return new NetInstance(Interop.NetInstance.Clone(Handle));
        }

        public void ActivateSignal(string signalName, params object[] parameters)
        {
            using (var list = new NetVariant())
            {
                foreach (var parameter in parameters)
                {
                    using (var variant = new NetVariant())
                    {
                        Helpers.PackValue(parameter, variant);
                    }
                }
                ActivateSignal(signalName, list);
            }
        }

        public void ActivateSignal(string signalName, params NetVariant[] paramters)
        {
            using (var list = new NetVariantList())
            {
                foreach (var variant in paramters)
                {
                    list.Add(variant);
                }
                ActivateSignal(signalName, list);
            }
        }

        public void ActivateSignal(string signalName, NetVariantList paramters)
        {
            Interop.NetInstance.ActivateSignal(Handle, signalName, paramters.Handle);
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

        public static bool ExistsForObject(object value)
        {
            return ObjectNetInstanceConnections.TryGetValue(value, out NetInstance netInstance);
        }
        
        public static NetInstance GetForObject(object value, bool autoCreate = true)
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

            if (!autoCreate) return null;
            
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
        [NativeSymbol(Entrypoint = "net_instance_clone")]
        IntPtr Clone(IntPtr instance);

        [NativeSymbol(Entrypoint = "net_instance_getHandle")]
        IntPtr GetHandle(IntPtr instance);
        [NativeSymbol(Entrypoint = "net_instance_activateSignal")]
        void ActivateSignal(IntPtr instance, [MarshalAs(UnmanagedType.LPWStr)]string signalName, IntPtr variants);
    }
}