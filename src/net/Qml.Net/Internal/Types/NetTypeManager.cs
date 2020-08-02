using System;
using System.Runtime.InteropServices;
using System.Security;

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
            if (type == null)
            {
                return null;
            }
            var result = Interop.NetTypeManager.GetTypeInfo(type.AssemblyQualifiedName);
            var netTypeInfo = result == IntPtr.Zero ? null : new NetTypeInfo(result);
            return netTypeInfo;
        }
    }

    internal class NetTypeManagerInterop
    {
        [NativeSymbol(Entrypoint = "type_manager_getTypeInfo")]
        public GetTypeInfoDel GetTypeInfo { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr GetTypeInfoDel([MarshalAs(UnmanagedType.LPWStr)]string fullTypeName);
    }
}