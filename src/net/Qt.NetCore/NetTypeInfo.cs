using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;

namespace Qt.NetCore
{
    public class NetTypeInfo : BaseDisposable
    {
        public NetTypeInfo(string fullTypeName)
            :base(Interop.NetTypeInfo.Create(fullTypeName))
        {
        }

        public string FullTypeName => Interop.NetTypeInfo.GetFullTypeName(Handle);
        
        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetTypeInfo.Destroy(ptr);
        }
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