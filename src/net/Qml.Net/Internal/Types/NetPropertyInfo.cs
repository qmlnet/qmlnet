using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Qml.Net.Internal.Types
{
    internal class NetPropertyInfo : BaseDisposable
    {
        public NetPropertyInfo(
            NetTypeInfo parentType,
            string name,
            NetTypeInfo returnType,
            bool canRead,
            bool canWrite,
            NetSignalInfo notifySignal)
            : this(Create(
                parentType,
                name,
                returnType,
                canRead,
                canWrite,
                notifySignal))
        {
        }

        public NetPropertyInfo(IntPtr handle, bool ownsHandle = true)
            : base(handle, ownsHandle)
        {
        }

        private static IntPtr Create(
            NetTypeInfo parentType,
            string name,
            NetTypeInfo returnType,
            bool canRead,
            bool canWrite,
            NetSignalInfo notifySignal)
        {
            return Interop.NetPropertyInfo.Create(
                parentType?.Handle ?? IntPtr.Zero,
                name,
                returnType?.Handle ?? IntPtr.Zero,
                canRead ? (byte)1 : (byte)0,
                canWrite ? (byte)1 : (byte)0,
                notifySignal?.Handle ?? IntPtr.Zero);
        }

        public int Id => Interop.NetPropertyInfo.GetId(Handle);

        public NetTypeInfo ParentType => new NetTypeInfo(Interop.NetPropertyInfo.GetParentType(Handle));

        public string Name => Utilities.ContainerToString(Interop.NetPropertyInfo.GetPropertyName(Handle));

        public NetTypeInfo ReturnType => new NetTypeInfo(Interop.NetPropertyInfo.GetReturnType(Handle));

        public bool CanRead => Interop.NetPropertyInfo.GetCanRead(Handle) == 1;

        public bool CanWrite => Interop.NetPropertyInfo.GetCanWrite(Handle) == 1;

        public NetSignalInfo NotifySignal
        {
            get
            {
                var result = Interop.NetPropertyInfo.GetNotifySignal(Handle);
                return result == IntPtr.Zero ? null : new NetSignalInfo(result);
            }

            set
            {
                Interop.NetPropertyInfo.SetNotifySignal(Handle, value.Handle);
            }
        }

        public void AddIndexParameter(string name, NetTypeInfo type)
        {
            Interop.NetPropertyInfo.AddIndexParameter(Handle, name, type.Handle);
        }

        public int IndexParameterCount => Interop.NetPropertyInfo.GetIndexParameterCount(Handle);

        public NetMethodInfoParameter GetIndexParameter(int index)
        {
            var result = Interop.NetPropertyInfo.GetIndexParameter(Handle, index);
            if (result == IntPtr.Zero) return null;
            return new NetMethodInfoParameter(result);
        }

        public List<NetMethodInfoParameter> GetAllIndexParameters()
        {
            var result = new List<NetMethodInfoParameter>();
            var count = IndexParameterCount;
            for (var x = 0; x < count; x++)
            {
                result.Add(GetIndexParameter(x));
            }
            return result;
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetPropertyInfo.Destroy(ptr);
        }
    }

    internal class NetPropertyInfoInterop
    {
        [NativeSymbol(Entrypoint = "property_info_create")]
        public CreateDel Create { get; set; }

        public delegate IntPtr CreateDel(
            IntPtr parentType,
            [MarshalAs(UnmanagedType.LPWStr)]string methodName,
            IntPtr returnType,
            byte canRead,
            byte canWrite,
            IntPtr notifySignal);

        [NativeSymbol(Entrypoint = "property_info_destroy")]
        public DestroyDel Destroy { get; set; }

        public delegate void DestroyDel(IntPtr property);

        [NativeSymbol(Entrypoint = "property_info_getId")]
        public GetIdDel GetId { get; set; }

        public delegate int GetIdDel(IntPtr property);

        [NativeSymbol(Entrypoint = "property_info_getParentType")]
        public GetParentTypeDel GetParentType { get; set; }

        public delegate IntPtr GetParentTypeDel(IntPtr property);

        [NativeSymbol(Entrypoint = "property_info_getPropertyName")]
        public GetPropertyNameDel GetPropertyName { get; set; }

        public delegate IntPtr GetPropertyNameDel(IntPtr property);

        [NativeSymbol(Entrypoint = "property_info_getReturnType")]
        public GetReturnTypeDel GetReturnType { get; set; }

        public delegate IntPtr GetReturnTypeDel(IntPtr property);

        [NativeSymbol(Entrypoint = "property_info_canRead")]
        public GetCanReadDel GetCanRead { get; set; }

        public delegate byte GetCanReadDel(IntPtr property);

        [NativeSymbol(Entrypoint = "property_info_canWrite")]
        public GetCanWriteDel GetCanWrite { get; set; }

        public delegate byte GetCanWriteDel(IntPtr property);

        [NativeSymbol(Entrypoint = "property_info_getNotifySignal")]
        public GetNotifySignalDel GetNotifySignal { get; set; }

        public delegate IntPtr GetNotifySignalDel(IntPtr property);

        [NativeSymbol(Entrypoint = "property_info_setNotifySignal")]
        public SetNotifySignalDel SetNotifySignal { get; set; }

        public delegate void SetNotifySignalDel(IntPtr property, IntPtr signal);

        [NativeSymbol(Entrypoint = "property_info_addIndexParameter")]
        public AddIndexParameterDel AddIndexParameter { get; set; }

        public delegate void AddIndexParameterDel(IntPtr method, [MarshalAs(UnmanagedType.LPWStr)]string name, IntPtr type);

        [NativeSymbol(Entrypoint = "property_info_getIndexParameterCount")]
        public GetIndexParameterCountDel GetIndexParameterCount { get; set; }

        public delegate int GetIndexParameterCountDel(IntPtr method);

        [NativeSymbol(Entrypoint = "property_info_getIndexParameter")]
        public GetIndexParameterDel GetIndexParameter { get; set; }

        public delegate IntPtr GetIndexParameterDel(IntPtr method, int index);
    }
}