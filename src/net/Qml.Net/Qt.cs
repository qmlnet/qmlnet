using AdvancedDLSupport;
using Qml.Net.Internal;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace Qml.Net
{
    public class Qt
    {
        public static bool PutEnv(string name, string value)
        {
            return Interop.QtInterop.PutEnv(name, value);
        }

        public static string GetEnv(string name)
        {
            return Utilities.ContainerToString(Interop.QtInterop.GetEnv(name));
        } 
    }

    internal interface IQtInterop
    {
        [NativeSymbol(Entrypoint = "qt_putenv")]
        bool PutEnv([MarshalAs(UnmanagedType.LPStr), CallerFree]string name, [MarshalAs(UnmanagedType.LPStr), CallerFree]string value);
        [NativeSymbol(Entrypoint = "qt_getenv")]
        IntPtr GetEnv(string name);
    }
}
