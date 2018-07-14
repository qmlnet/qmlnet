using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Qt.NetCore.Sandbox
{
    class Program
    {
        static int Main()
        {
            System.Console.WriteLine("Test");
            var callbacks = new CallbacksImpl();
            Interop.Callbacks.registerCallbacks();
        }
    }
}
