using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;

namespace Qml.Net.Internal.Types
{
    internal class NetTypeManager
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
    }
    
    internal interface INetTypeManagerInterop
    {
        [NativeSymbol(Entrypoint = "type_manager_getTypeInfo")]
        IntPtr GetTypeInfo([MarshalAs(UnmanagedType.LPWStr)]string fullTypeName);
    }
}