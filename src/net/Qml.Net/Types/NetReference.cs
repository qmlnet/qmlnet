using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qml.Net.Internal;
using Qml.Net.Qml;

namespace Qml.Net.Types
{
    public class NetReference : BaseDisposable
    {
        private NetReference(IntPtr gcHandle, NetTypeInfo type, bool ownsHandle = true)
            :base(Interop.NetReference.Create(gcHandle, type.Handle), ownsHandle)
        {
        }

        public NetReference(IntPtr handle, bool ownsHandle = true)
            : base(handle, ownsHandle)
        {
            
        }

        public object Instance
        {
            get
            {
                var handle = Interop.NetReference.GetHandle(Handle);
                return ((GCHandle) handle).Target;
            }
        }

        public NetReference Clone()
        {
            return new NetReference(Interop.NetReference.Clone(Handle));
        }

        public bool ActivateSignal(string signalName, params object[] parameters)
        {
            if (parameters != null && parameters.Length > 0)
            {
                using (var list = new NetVariantList())
                {
                    foreach (var parameter in parameters)
                    {
                        using (var variant = new NetVariant())
                        {
                            Helpers.PackValue(parameter, variant);
                            list.Add(variant);
                        }
                    }
                    return Interop.NetReference.ActivateSignal(Handle, signalName, list.Handle);
                }
            }
            else
            {
                return Interop.NetReference.ActivateSignal(Handle, signalName, IntPtr.Zero);
            }
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetReference.Destroy(ptr);
        }
        
        #region Instance helpers
        
        public static ITypeCreator TypeCreator { get; set; }
        
        private static Type GetUnproxiedType(Type type)
        {
            if (type.Namespace == "Castle.Proxies")
                return type.BaseType;

            return type;
        }
        
        private static readonly ConditionalWeakTable<object, NetReference> ObjectNetReferenceConnections = new ConditionalWeakTable<object, NetReference>();

        public static bool ExistsForObject(object value)
        {
            return ObjectNetReferenceConnections.TryGetValue(value, out NetReference NetReference);
        }
        
        public static NetReference GetForObject(object value, bool autoCreate = true)
        {
            if (value == null) return null;
            var alreadyExists = false;
            if (ObjectNetReferenceConnections.TryGetValue(value, out var NetReference))
            {
                alreadyExists = true;
                if (GCHandle.FromIntPtr(NetReference.Handle).IsAllocated)
                {
                    return NetReference;
                }
            }

            if (!autoCreate) return null;
            
            var typeInfo = NetTypeManager.GetTypeInfo(GetUnproxiedType(value.GetType()).AssemblyQualifiedName);
            if(typeInfo == null) throw new InvalidOperationException($"Couldn't create type info from {value.GetType().AssemblyQualifiedName}");
            var handle = GCHandle.Alloc(value);
            var newNetReference = new NetReference(GCHandle.ToIntPtr(handle), typeInfo);
            if(alreadyExists)
            {
                ObjectNetReferenceConnections.Remove(value);
            }
            ObjectNetReferenceConnections.Add(value, newNetReference);
            return newNetReference;
        }
        
        #endregion
    }

    public interface INetReferenceInterop
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
        bool ActivateSignal(IntPtr instance, [MarshalAs(UnmanagedType.LPWStr)]string signalName, IntPtr variants);
    }
}