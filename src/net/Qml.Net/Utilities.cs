using System;
using System.Runtime.InteropServices;
using Qml.Net.Internal;

namespace Qml.Net
{
    internal class Utilities
    {
        public static string ContainerToString(IntPtr container)
        {
            if (container == IntPtr.Zero)
            {
                return null;
            }

            var containerStruct = Marshal.PtrToStructure<StringContainer>(container);
            var result = Marshal.PtrToStringUni(containerStruct.Data);
            Interop.Utilities.FreeString(container);
            return result;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct StringContainer
        {
            private readonly IntPtr _ignore;
            public readonly IntPtr Data;
        }
    }

    internal class UtilitiesInterop
    {
        [NativeSymbol(Entrypoint = "freeString")]
        public FreeStringDel FreeString { get; set; }

        public delegate void FreeStringDel(IntPtr container);
    }
}