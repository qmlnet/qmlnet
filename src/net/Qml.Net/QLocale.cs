using System;
using System.Runtime.InteropServices;
using System.Security;
using Qml.Net.Internal;

namespace Qml.Net
{
    public class QLocale
    {
        public static string SetDefault(string name)
        {
            return Utilities.ContainerToString(Interop.QLocale.SetDefaultName(name));
        }
    }

    internal class QLocaleInterop
    {
        [NativeSymbol(Entrypoint = "qlocale_set_default_name")]
        public SetDefaultNameDef SetDefaultName { get; set; }

        [SuppressUnmanagedCodeSecurity]
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate IntPtr SetDefaultNameDef([MarshalAs(UnmanagedType.LPStr)]string name);
    }
}