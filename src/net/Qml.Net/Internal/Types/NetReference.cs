using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Qml.Net.Internal.Qml;

namespace Qml.Net.Internal.Types
{
    internal class NetReference : BaseDisposable
    {
        private NetReference(UInt64 objectId, NetTypeInfo type, bool ownsHandle = true)
            : base(Interop.NetReference.Create(objectId, type.Handle), ownsHandle)
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
                if (ObjectIdReferenceTracker.TryGetObjectFor(ObjectId, out var obj))
                {
                    return obj;
                }
                throw new InvalidOperationException($"No object found for object id {ObjectId}");
            }
        }

        public ulong ObjectId => Interop.NetReference.GetObjectId(Handle);

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
                    return Interop.NetReference.ActivateSignal(Handle, signalName, list.Handle) == 1;
                }
            }
            else
            {
                return Interop.NetReference.ActivateSignal(Handle, signalName, IntPtr.Zero) == 1;
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

        public static NetReference CreateForObject(object value, bool autoCreateIfNotExist = true)
        {
            if (value == null) return null;

            var objectId = value.GetTag();
            if (!autoCreateIfNotExist && !objectId.HasValue)
            {
                // This item isn't tagged, so don't auto tag.
                return null;
            }

            var typeInfo = NetTypeManager.GetTypeInfo(GetUnproxiedType(value.GetType()));
            if (typeInfo == null)
            {
                throw new InvalidOperationException($"Couldn't create type info from {value.GetType().AssemblyQualifiedName}");
            }

            objectId = value.GetOrCreateTag();
            var newNetReference = new NetReference(objectId.Value, typeInfo);

            ObjectIdReferenceTracker.OnReferenceCreated(value, objectId.Value);

            return newNetReference;
        }

        public static void OnRelease(UInt64 objectId)
        {
            ObjectIdReferenceTracker.OnReferenceReleased(objectId);
        }

        #endregion
    }

    internal class NetReferenceInterop
    {
        [NativeSymbol(Entrypoint = "net_instance_create")]
        public CreateDel Create { get; set; }

        public delegate IntPtr CreateDel(UInt64 objectId, IntPtr type);

        [NativeSymbol(Entrypoint = "net_instance_destroy")]
        public DestroyDel Destroy { get; set; }

        public delegate void DestroyDel(IntPtr instance);

        [NativeSymbol(Entrypoint = "net_instance_clone")]
        public CloneDel Clone { get; set; }

        public delegate IntPtr CloneDel(IntPtr instance);

        [NativeSymbol(Entrypoint = "net_instance_getObjectId")]
        public GetObjectIdDel GetObjectId { get; set; }

        public delegate UInt64 GetObjectIdDel(IntPtr instance);

        [NativeSymbol(Entrypoint = "net_instance_activateSignal")]
        public ActivateSignalDel ActivateSignal { get; set; }

        public delegate byte ActivateSignalDel(IntPtr instance, [MarshalAs(UnmanagedType.LPWStr)]string signalName, IntPtr variants);
    }

    internal static class ObjectIdReferenceTracker
    {
        class ObjectEntry
        {
            public object Obj { get; private set; }

            public UInt64 Counter { get; set; }

            public ObjectEntry(object obj, UInt64 count)
            {
                Obj = obj;
                Counter = count;
            }
        }

        private static Dictionary<UInt64, ObjectEntry> _ObjectIdObjectLookup = new Dictionary<ulong, ObjectEntry>();
        private static object _LockObject = new object();

        internal static bool TryGetObjectFor(UInt64 objectId, out object obj)
        {
            lock (_LockObject)
            {
                if (_ObjectIdObjectLookup.ContainsKey(objectId))
                {
                    obj = _ObjectIdObjectLookup[objectId].Obj;
                    return true;
                }
                obj = null;
                return false;
            }
        }

        internal static void OnReferenceReleased(UInt64 objectId)
        {
            lock (_LockObject)
            {
                if (!_ObjectIdObjectLookup.ContainsKey(objectId))
                {
                    throw new InvalidOperationException("Releasing a NetReference that hasn't been counted!");
                }
                _ObjectIdObjectLookup[objectId].Counter--;

                // When there are no more QML references.
                if (_ObjectIdObjectLookup[objectId].Counter == 0)
                {
                    var obj = _ObjectIdObjectLookup[objectId].Obj;

                    // Remove object entry.
                    _ObjectIdObjectLookup.Remove(objectId);

                    // And notify the behaviors.
                    InteropBehaviors.OnObjectLeavesNative(obj, objectId);
                }
            }
        }

        internal static void OnReferenceCreated(object obj, UInt64 objectId)
        {
            lock (_LockObject)
            {
                if (!_ObjectIdObjectLookup.ContainsKey(objectId))
                {
                    _ObjectIdObjectLookup[objectId] = new ObjectEntry(obj, 1);
                    InteropBehaviors.OnObjectEntersNative(obj, objectId);
                }
                else
                {
                    _ObjectIdObjectLookup[objectId].Counter++;
                }
            }
        }
    }
}