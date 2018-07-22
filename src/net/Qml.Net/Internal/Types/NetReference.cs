using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using AdvancedDLSupport;
using Qml.Net.Internal;
using Qml.Net.Internal.Qml;

namespace Qml.Net.Internal.Types
{
    internal class NetReference : BaseDisposable
    {
        private NetReference(IntPtr gcHandle, UInt64 objectId, NetTypeInfo type, bool ownsHandle = true)
            :base(Interop.NetReference.Create(gcHandle, objectId, type.Handle), ownsHandle)
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

        public ulong ObjectId
        {
            get
            {
                return Interop.NetReference.GetObjectId(Handle);
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
        
        private static Type GetUnproxiedType(Type type)
        {
            if (type.Namespace == "Castle.Proxies")
                return type.BaseType;

            return type;
        }
        
        public static NetReference CreateForObject(object value)
        {
            if (value == null) return null;

            var typeInfo = NetTypeManager.GetTypeInfo(GetUnproxiedType(value.GetType()).AssemblyQualifiedName);
            if(typeInfo == null) throw new InvalidOperationException($"Couldn't create type info from {value.GetType().AssemblyQualifiedName}");
            var handle = GCHandle.Alloc(value);

            var objectId = value.GetOrCreateTag();
            var newNetReference = new NetReference(GCHandle.ToIntPtr(handle), objectId, typeInfo);

            return newNetReference;
        }
        
        #endregion
    }

    internal interface INetReferenceInterop
    {   
        [NativeSymbol(Entrypoint = "net_instance_create")]
        IntPtr Create(IntPtr handle, UInt64 objectId, IntPtr type);
        [NativeSymbol(Entrypoint = "net_instance_destroy")]
        void Destroy(IntPtr instance);
        [NativeSymbol(Entrypoint = "net_instance_clone")]
        IntPtr Clone(IntPtr instance);

        [NativeSymbol(Entrypoint = "net_instance_getHandle")]
        IntPtr GetHandle(IntPtr instance);
        [NativeSymbol(Entrypoint = "net_instance_getObjectId")]
        UInt64 GetObjectId(IntPtr instance);
        [NativeSymbol(Entrypoint = "net_instance_activateSignal")]
        bool ActivateSignal(IntPtr instance, [MarshalAs(UnmanagedType.LPWStr)]string signalName, IntPtr variants);
    }
}