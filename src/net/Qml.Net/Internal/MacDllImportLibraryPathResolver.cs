using System;
using System.IO;
using System.Runtime.InteropServices;
using AdvancedDLSupport;

namespace Qml.Net.Internal
{
    public class MacDllImportLibraryPathResolver : ILibraryPathResolver
    {
        ILibraryPathResolver _original;

        public MacDllImportLibraryPathResolver(ILibraryPathResolver original)
        {
            _original = original;
        }

        public ResolvePathResult Resolve(string library)
        {
            var result = _original.Resolve(library);

            if(!result.IsSuccess && library == "QmlNet")
            {
                // Try to let .NET load the library.
                try
                {
                    qml_net_getVersion();

                    // The method invoked correctly, so .NET loaded it.
                    // Let's return the path to it.
                    var dll = dlopen("libQmlNet.dylib", SymbolFlag.RtldLazy);
                    if (dll == IntPtr.Zero)
                    {
                        return result;
                    }
                    
                    var sym = dlsym(dll, "qml_net_getVersion");
                    if (sym == IntPtr.Zero)
                    {
                        return result;
                    }

                    
                    var info = new DlInfo();
                    if (dladdr(sym, ref info) != 1)
                    {
                        return result;
                    }

                    var location = Marshal.PtrToStringAnsi(info.fname);

                    if(File.Exists(location))
                    {
                        return ResolvePathResult.FromSuccess(location);
                    }
                }
                // ReSharper disable EmptyGeneralCatchClause
                catch (Exception)
                // ReSharper restore EmptyGeneralCatchClause
                {
                }
            }

            return result;
        }

        [DllImport("libQmlNet.dylib")]
        static extern long qml_net_getVersion();
        
        [DllImport("dl")]
        static extern IntPtr dlopen(string fileName, SymbolFlag flags);
        
        [DllImport("dl")]
        static extern IntPtr dlsym(IntPtr handle, string name);
        
        [DllImport("dl")]
        static extern int dladdr(IntPtr handle, ref DlInfo info);

        private struct DlInfo
        {
            public IntPtr fname;
            private IntPtr notUsed1;
            private IntPtr notUsed2;
            private IntPtr notUsed3;
        }
        
        [Flags]
        private enum SymbolFlag
        {
            RtldLazy = 0x00001
        }
    }
}