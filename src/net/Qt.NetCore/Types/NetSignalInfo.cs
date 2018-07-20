using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qt.NetCore.Internal;

namespace Qt.NetCore.Types
{
    public class NetSignalInfo : BaseDisposable
    {
        internal NetSignalInfo(IntPtr handle, bool ownsHandle = true)
            : base(handle, ownsHandle)
        {

        }

        public NetSignalInfo(string name)
            : this(Interop.NetSignalInfo.Create(name))
        {

        }

        public string Name => Interop.NetSignalInfo.GetName(Handle);
        
        public void AddParameter(NetVariantType type)
        {
            Interop.NetSignalInfo.AddParameter(Handle, type);
        }

        public uint ParameterCount => Interop.NetSignalInfo.GetParameterCount(Handle);

        public NetVariantType GetParameter(uint index)
        {
            return Interop.NetSignalInfo.GetParameter(Handle, index);
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetSignalInfo.Destroy(ptr);
        }
    }

    public interface INetSignalInfoInterop
    {
        [NativeSymbol(Entrypoint = "signal_info_create")]
        IntPtr Create([MarshalAs(UnmanagedType.LPWStr)] string name);
        [NativeSymbol(Entrypoint = "signal_info_destroy")]
        void Destroy(IntPtr signal);

        [NativeSymbol(Entrypoint = "signal_info_getName")]
        [return: MarshalAs(UnmanagedType.LPWStr)]
        string GetName(IntPtr signal);
        
        [NativeSymbol(Entrypoint = "signal_info_addParameter")]
        void AddParameter(IntPtr signal, NetVariantType type);
        [NativeSymbol(Entrypoint = "signal_info_getParameterCount")]
        uint GetParameterCount(IntPtr signal);
        [NativeSymbol(Entrypoint = "signal_info_getParameter")]
        NetVariantType GetParameter(IntPtr signal, uint index);
    }
}