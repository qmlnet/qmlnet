using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qml.Net.Internal;

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

    internal interface INetSignalInfoInterop
    {
        [NativeSymbol(Entrypoint = "signal_info_create")]
        IntPtr Create(IntPtr parentType, [MarshalAs(UnmanagedType.LPWStr), CallerFree] string name);
        [NativeSymbol(Entrypoint = "signal_info_destroy")]
        void Destroy(IntPtr signal);

        [NativeSymbol(Entrypoint = "signal_info_getParentType")]
        IntPtr GetParentType(IntPtr signal);
        [NativeSymbol(Entrypoint = "signal_info_getName")]
        IntPtr GetName(IntPtr signal);
        
        [NativeSymbol(Entrypoint = "signal_info_addParameter")]
        void AddParameter(IntPtr signal, NetVariantType type);
        [NativeSymbol(Entrypoint = "signal_info_getParameterCount")]
        int GetParameterCount(IntPtr signal);
        [NativeSymbol(Entrypoint = "signal_info_getParameter")]
        NetVariantType GetParameter(IntPtr signal, int index);
    }
}