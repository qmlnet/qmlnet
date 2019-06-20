using System;
using Qml.Net;

namespace NetHost
{
    class Program
    {
        public class TestObject
        {
            public void TestMethod()
            {
                Console.WriteLine("test method");
            }
        }

        static int Main(string[] _)
        {
            // The "_" contains some private arguements to help
            // bootstrap things. It is intended to be passed
            // immediately into Host.Run(...).
            
            // Phase 5
            // Unwrap the magic.
            return Host.Run(_, (args, app, engine, runCallback) =>
            {
                // "args" contains the any user defined arguements passed from
                // CoreHost::run(..) in C++.
                
                // Phase 6
                // Register any .NET types that will be used.
                Qml.Net.Qml.RegisterType<TestObject>("test", 1, 0);

                // Phase 7
                // This callback passes control back to C++ to perform
                // any file registrations and run the event loop.
                return runCallback();
            });
        }
    }
}
