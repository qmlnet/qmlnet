using System;
using System.Runtime.InteropServices;

namespace Qml.Net.Internal.Types
{
    internal class NetSignalInfo : BaseDisposable
    {
        internal NetSignalInfo(IntPtr handle, bool ownsHandle = true)
            : base(handle, ownsHandle)
        {

        }

        public NetSignalInfo(NetTypeInfo parentType, string name)
            : this(Interop.NetSignalInfo.Create(parentType.Handle, name))
        {

        }

        public NetTypeInfo ParentType => new NetTypeInfo(Interop.NetSignalInfo.GetParentType(Handle));

        public string Name => Utilities.ContainerToString(Interop.NetSignalInfo.GetName(Handle));
        
        public void AddParameter(NetVariantType type)
        {
            Interop.NetSignalInfo.AddParameter(Handle, type);
        }

        public int ParameterCount => Interop.NetSignalInfo.GetParameterCount(Handle);

        public NetVariantType GetParameter(int index)
        {
            return Interop.NetSignalInfo.GetParameter(Handle, index);
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetSignalInfo.Destroy(ptr);
        }
    }

    internal class NetSignalInfoInterop
    {
        [NativeSymbol(Entrypoint = "signal_info_create")]
        public CreateDel Create { get; set; }
        public delegate IntPtr CreateDel(IntPtr parentType, [MarshalAs(UnmanagedType.LPWStr)] string name);
        [NativeSymbol(Entrypoint = "signal_info_destroy")]
        public DestroyDel Destroy { get; set; }
        public delegate void DestroyDel(IntPtr signal);

        [NativeSymbol(Entrypoint = "signal_info_getParentType")]
        public GetParentTypeDel GetParentType { get; set; }
        public delegate IntPtr GetParentTypeDel(IntPtr signal);
        [NativeSymbol(Entrypoint = "signal_info_getName")]
        public GetNameDel GetName { get; set; }
        public delegate IntPtr GetNameDel(IntPtr signal);
        
        [NativeSymbol(Entrypoint = "signal_info_addParameter")]
        public AddParameterDel AddParameter { get; set; }
        public delegate void AddParameterDel(IntPtr signal, NetVariantType type);
        [NativeSymbol(Entrypoint = "signal_info_getParameterCount")]
        public GetParameterCountDel GetParameterCount { get; set; }
        public delegate int GetParameterCountDel(IntPtr signal);
        [NativeSymbol(Entrypoint = "signal_info_getParameter")]
        public GetParameterDel GetParameter { get; set; }
        public delegate NetVariantType GetParameterDel(IntPtr signal, int index);
    }
}