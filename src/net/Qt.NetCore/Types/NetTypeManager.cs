using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;

namespace Qt.NetCore.Types
{
    public class NetTypeManager
    {
        public static NetTypeInfo GetTypeInfo<T>()
        {
            return GetTypeInfo(typeof(T).AssemblyQualifiedName);
        }

        public static NetTypeInfo GetTypeInfo(string fullTypeName)
        {
            var result = Interop.NetTypeManager.GetTypeInfo(fullTypeName);
            return result == IntPtr.Zero ? null : new NetTypeInfo(result);
        }

        public static NetInstance InstantiateType(NetTypeInfo type)
        {
            var result = Interop.Callbacks.InstantiateType(type.Handle);
            if (result == IntPtr.Zero) return null;
            return new NetInstance(result);
        }
    }
    
    public interface INetTypeManagerInterop
    {
        [NativeSymbol(Entrypoint = "type_manager_getTypeInfo")]
        IntPtr GetTypeInfo([MarshalAs(UnmanagedType.LPStr)]string fullTypeName);
    }
}