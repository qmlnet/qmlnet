using System;
using System.IO;
using System.Runtime.InteropServices;
using NetNativeLibLoader.PathResolver;

namespace Qml.Net.Internal
{
    internal class WindowsDllImportLibraryPathResolver : IPathResolver
    {
        IPathResolver _original;

        public WindowsDllImportLibraryPathResolver(IPathResolver original)
        {
            _original = original;
        }

        public ResolvePathResult Resolve(string library)
        {
            var result = _original.Resolve(library);

            if (!result.IsSuccess && library == "QmlNet")
            {
                // Try to let .NET load the library.
                try
                {
                    qml_net_getVersion();

                    // The method invoked correctly, so .NET loaded it.
                    // Let's return the path to it.
                    var loaded = GetModuleHandle("QmlNet");

                    var bytes = Marshal.AllocHGlobal(2000);
                    var r = GetModuleFileName(loaded, bytes, 2000);
                    var path = Marshal.PtrToStringAnsi(bytes);
                    Marshal.FreeHGlobal(bytes);

                    if (File.Exists(path))
                    {
                        return ResolvePathResult.FromSuccess(path);
                    }
                }
                catch (Exception)
                {
                }
            }

            return result;
        }

        [DllImport("QmlNet")]
        internal static extern long qml_net_getVersion();

        [DllImport("kernel32.dll")]
        public static extern uint GetModuleFileName([In]IntPtr hModule, [Out]IntPtr lpFilename, [In, MarshalAs(UnmanagedType.U4)]int nSize);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        public static extern IntPtr GetModuleHandle(string lpModuleName);
    }
}
