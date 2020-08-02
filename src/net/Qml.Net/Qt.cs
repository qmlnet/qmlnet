using System;
using System.Runtime.InteropServices;
using System.Security;
using Qml.Net.Internal;
using Qml.Net.Internal.Qml;

namespace Qml.Net
{
    public class Qt
    {
        public static bool PutEnv(string name, string value)
        {
            return Interop.QtInterop.PutEnv(name, value) == 1;
        }

        public static string GetEnv(string name)
        {
            return Utilities.ContainerToString(Interop.QtInterop.GetEnv(name));
        }

        public static Version GetQtVersion()
        {
            return Version.Parse(Utilities.ContainerToString(Interop.QtInterop.QtVersion()));
        }

        public static INetQObject BuildQObject(string className)
        {
            return NetQObject.BuildQObject(className)?.AsDynamic();
        }
    }

    internal class QtInterop
    {
        [NativeSymbol(Entrypoint = "qt_putenv")]
        public PutEnvDel PutEnv { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate byte PutEnvDel([MarshalAs(UnmanagedType.LPStr)]string name, [MarshalAs(UnmanagedType.LPStr)]string value);

        [NativeSymbol(Entrypoint = "qt_getenv")]
        public GetEnvDel GetEnv { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr GetEnvDel(string name);

        [NativeSymbol(Entrypoint = "qt_version")]
        public QtVersionDel QtVersion { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr QtVersionDel();
    }
}
