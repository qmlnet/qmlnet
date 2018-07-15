using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;
using Qt.NetCore.Internal;

namespace Qt.NetCore
{
    public class NetMethodInfo : BaseDisposable
    {
        public NetMethodInfo(NetTypeInfo parentTypeInfo,
            string methodName,
            NetTypeInfo returnTypeInfo)
            : this(Create(parentTypeInfo, methodName, returnTypeInfo))
        {
            
        }

        public NetMethodInfo(IntPtr handle, bool ownsHandle = true)
            :base(handle, ownsHandle)
        {
            
        }

        private static IntPtr Create(NetTypeInfo parentTypeInfo,
            string methodName,
            NetTypeInfo returnTypeInfo)
        {
            return Interop.NetMethodInfo.Create(parentTypeInfo?.Handle ?? IntPtr.Zero,
                methodName,
                returnTypeInfo?.Handle ?? IntPtr.Zero);
        }

        protected override void DisposeUnmanaged(IntPtr ptr)
        {
            Interop.NetMethodInfo.Destroy(ptr);
        }
    }
    
    public interface INetMethodInfoInterop
    {
        [NativeSymbol(Entrypoint = "method_info_create")]
        IntPtr Create(IntPtr parentTypeInfo, [MarshalAs(UnmanagedType.LPStr)]string methodName, IntPtr returnTypeInfo);
        [NativeSymbol(Entrypoint = "method_info_destroy")]
        void Destroy(IntPtr methodInfo);
    }
}