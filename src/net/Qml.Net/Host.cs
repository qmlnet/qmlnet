using System;
using System.Linq;
using System.Runtime.InteropServices;

namespace Qml.Net
{
    public static class Host
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int NativeRunCallbackDelegate(IntPtr app, IntPtr engine);

        public delegate int NetRunCallbackDelegate();
        
        public static int Run(string[] args, Func<string[], QGuiApplication, QQmlApplicationEngine, NetRunCallbackDelegate, int> action)
        {
            if (args.Length < 3)
            {
                throw new Exception("Args is invalid, must contain three entries which are pointers to native types.");
            }
            
            var appPtr = new IntPtr((long)ulong.Parse(args[0]));
            var enginePtr = new IntPtr((long)ulong.Parse(args[1]));
            var callbackPtr = new IntPtr((long)ulong.Parse(args[2]));

            using (var app = new QGuiApplication(appPtr))
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
    }
}