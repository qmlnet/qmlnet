using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qt.NetCore.Internal;

namespace Qt.NetCore.Types
{
    public class NetPropertyInfo : BaseDisposable
    {
        public NetPropertyInfo(NetTypeInfo parentType,
            string name,
            NetTypeInfo returnType,
            bool canRead,
            bool canWrite,
            NetSignalInfo notifySignal)
            : this(Create(parentType,
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

        private static IntPtr Create(NetTypeInfo parentType,
            string name,
            NetTypeInfo returnType,
            bool canRead,
            bool canWrite,
            NetSignalInfo notifySignal)
        {
            return Interop.NetPropertyInfo.Create(parentType?.Handle ?? IntPtr.Zero,
                name,
                returnType?.Handle ?? IntPtr.Zero,
                canRead,
                canWrite,
                notifySignal?.Handle ?? IntPtr.Zero);
        }

        public NetTypeInfo ParentType => new NetTypeInfo(Interop.NetPropertyInfo.GetParentType(Handle));

        public string Name => Interop.NetPropertyInfo.GetPropertyName(Handle);
        
        public NetTypeInfo ReturnType => new NetTypeInfo(Interop.NetPropertyInfo.GetReturnType(Handle));

        public bool CanRead => Interop.NetPropertyInfo.GetCanRead(Handle);

        public bool CanWrite => Interop.NetPropertyInfo.GetCanWrite(Handle);

        public NetSignalInfo NotifySignal
        {
            get
            {
                var result = Interop.NetPropertyInfo.GetNotifySignal(Handle);
                return result == IntPtr.Zero ? null : new NetSignalInfo(result);
            }
        }
        
        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetPropertyInfo.Destroy(ptr);
        }
    }

    public interface INetPropertyInfoInterop
    {
        [NativeSymbol(Entrypoint = "property_info_create")]
        IntPtr Create(IntPtr parentType,
            [MarshalAs(UnmanagedType.LPWStr)]string methodName,
            IntPtr returnType,
            bool canRead,
            bool canWrite,
            IntPtr notifySignal);
        [NativeSymbol(Entrypoint = "property_info_destroy")]
        void Destroy(IntPtr property);

        [NativeSymbol(Entrypoint = "property_info_getParentType")]
        IntPtr GetParentType(IntPtr property);

        [NativeSymbol(Entrypoint = "property_info_getPropertyName")]
        [return:MarshalAs(UnmanagedType.LPWStr)]string GetPropertyName(IntPtr property);

        [NativeSymbol(Entrypoint = "property_info_getReturnType")]
        IntPtr GetReturnType(IntPtr property);

        [NativeSymbol(Entrypoint = "property_info_canRead")]
        bool GetCanRead(IntPtr property);

        [NativeSymbol(Entrypoint = "property_info_canWrite")]
        bool GetCanWrite(IntPtr property);

        [NativeSymbol(Entrypoint = "property_info_getNotifySignal")]
        IntPtr GetNotifySignal(IntPtr property);
    }
}