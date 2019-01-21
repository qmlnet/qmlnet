using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using NetNativeLibLoader.Loader;
using NetNativeLibLoader.PathResolver;
using Qml.Net.Internal;

namespace Qml.Net
{
    public static class Host
    {
        internal class Loader : IPlatformLoader, IPathResolver
        {
            public T LoadFunction<T>(IntPtr library, string symbolName)
            {
                IntPtr symbol = GetExportedSymbol(symbolName);
                return Marshal.GetDelegateForFunctionPointer<T>(symbol);
            }

            public IntPtr LoadLibrary(string path)
            {
                return IntPtr.Zero;
            }

            public IntPtr LoadSymbol(IntPtr library, string symbolName)
            {
                return GetExportedSymbol(symbolName);
            }

            public bool CloseLibrary(IntPtr library)
            {
                return true;
            }

            public ResolvePathResult Resolve(string library)
            {
                return ResolvePathResult.FromSuccess(library);
            }
        }

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate IntPtr GetExportedSymbolDelegate([MarshalAs(UnmanagedType.LPStr)]string symbolName);

        internal static GetExportedSymbolDelegate GetExportedSymbol;

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate int NativeRunCallbackDelegate(IntPtr app, IntPtr engine);

        public delegate int NetRunCallbackDelegate();

        public static int Run(string[] args, Func<string[], QCoreApplication, QQmlApplicationEngine, NetRunCallbackDelegate, int> action)
        {
            if (args.Length < 4)
            {
                throw new Exception("Args is invalid, must contain three entries which are pointers to native types.");
            }

            var appPtr = new IntPtr((long)ulong.Parse(args[0]));
            var enginePtr = new IntPtr((long)ulong.Parse(args[1]));
            var callbackPtr = new IntPtr((long)ulong.Parse(args[2]));
            var exportedSymbolPtr = new IntPtr((long)ulong.Parse(args[3]));
            GetExportedSymbol = Marshal.GetDelegateForFunctionPointer<GetExportedSymbolDelegate>(exportedSymbolPtr);

            QCoreApplication app = null;
            switch (Interop.QCoreApplication.GetAppType(IntPtr.Zero, appPtr))
            {
                case 0:
                    app = new QCoreApplication(appPtr);
                    break;
                case 1:
                    app = new QGuiApplication(appPtr);
                    break;
                case 2:
                    app = new QApplication(appPtr);
                    break;
                default:
                    throw new Exception("Invalid app type");
            }

            using (app)
            {
                using (var engine = new QQmlApplicationEngine(enginePtr))
                {
                    var runCallback = new NetRunCallbackDelegate(() =>
                    {
                        var callback = Marshal.GetDelegateForFunctionPointer<NativeRunCallbackDelegate>(callbackPtr);
                        // ReSharper disable AccessToDisposedClosure
                        return callback.Invoke(app.InternalPointer, engine.InternalPointer);
                        // ReSharper restore AccessToDisposedClosure
                    });
                    return action(args.Skip(2).ToArray(), app, engine, runCallback);
                }
            }
        }

        public static Task<int> RunAsync(string[] _,
            Func<string[], QCoreApplication, QQmlApplicationEngine, NetRunCallbackDelegate, Task<int>> action)
        {
            var exitCode = Run(_, (args, application, engine, callback) =>
            {
                var result = action(args, application, engine, callback);
                return result.GetAwaiter().GetResult();
            });
            return Task.FromResult(exitCode);
        }
    }
}