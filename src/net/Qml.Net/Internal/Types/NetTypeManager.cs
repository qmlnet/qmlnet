using System;
using System.Runtime.InteropServices;
using AdvancedDLSupport;

namespace Qml.Net.Internal.Types
{
    internal class NetTypeManager
    {
        public static NetTypeInfo GetTypeInfo<T>()
        {
            return GetTypeInfo(typeof(T));
        }

        public static NetTypeInfo GetTypeInfo(Type type)
        {
            if(type == null)
            {
                return null;
            }
            var result = Interop.NetTypeManager.GetTypeInfo(type.AssemblyQualifiedName);
            var netTypeInfo = result == IntPtr.Zero ? null : new NetTypeInfo(result);
            return netTypeInfo;
        }
    }
    
    internal interface INetTypeManagerInterop
    {
        [NativeSymbol(Entrypoint = "type_manager_getTypeInfo")]
        IntPtr GetTypeInfo([MarshalAs(UnmanagedType.LPWStr)]string fullTypeName);
    }
}