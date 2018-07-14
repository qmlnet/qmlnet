using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;

namespace Qt.NetCore
{
    public class NetTypeInfo
    {
        readonly IntPtr _handle;
        
        public NetTypeInfo(string netTypeInfo)
        {
            _handle = Interop.NetTypeInfo.Create(netTypeInfo);
        }

        public string FullTypeName => Interop.NetTypeInfo.GetFullTypeName(_handle);
    }

    public interface INetTypeInfoInterop
    {
        [NativeSymbol(Entrypoint = "type_info_create")]
        IntPtr Create([MarshalAs(UnmanagedType.LPStr)]string fullTypeName);
        [NativeSymbol(Entrypoint = "type_info_destroy")]
        void Destroy(IntPtr netTypeInfo);
        [NativeSymbol(Entrypoint = "type_info_getFullTypeName")]
        [return: MarshalAs(UnmanagedType.LPStr)]string GetFullTypeName(IntPtr netTypeInfo);
    }
}