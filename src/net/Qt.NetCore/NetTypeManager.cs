using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;

namespace Qt.NetCore
{
    public class NetTypeManager
    {
        public static NetTypeInfo GetTypeInfo<T>()
        {
            var result = Interop.NetTypeManager.GetTypeInfo(typeof(T).AssemblyQualifiedName);
            return result == IntPtr.Zero ? null : new NetTypeInfo(result);
        }
    }
    
    public interface INetTypeManagerInterop
    {
        [NativeSymbol(Entrypoint = "type_manager_getTypeInfo")]
        IntPtr GetTypeInfo([MarshalAs(UnmanagedType.LPStr)]string fullTypeName);
    }
}