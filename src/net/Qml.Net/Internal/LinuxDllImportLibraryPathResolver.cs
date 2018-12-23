using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using NetNativeLibLoader.PathResolver;

namespace Qml.Net.Internal
{
    public class LinuxDllImportLibraryPathResolver : IPathResolver
    {
        IPathResolver _original;

        public LinuxDllImportLibraryPathResolver(IPathResolver original)
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

                    using (var currentProcess = Process.GetCurrentProcess())
                    {
                        foreach (ProcessModule module in currentProcess.Modules)
                        {
                            if (Path.GetFileNameWithoutExtension(module.FileName) == "libQmlNet")
                            {
                                return ResolvePathResult.FromSuccess(module.FileName);
                            }
                        }
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

        [DllImport("libQmlNet.so")]
        static extern long qml_net_getVersion();
    }
}