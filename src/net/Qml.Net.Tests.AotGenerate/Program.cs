using System;
using System.IO;
using Qml.Net.Aot;

namespace Qml.Net.Tests.AotGenerate
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var session = new AotSession("test-interop"))
            {
                foreach (var type in typeof(Aot.AotTestsBase).Assembly.GetTypes())
                {
                    if (typeof(Aot.AotTestsBase).IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        var instance = Activator.CreateInstance(type) as Aot.AotTestsBase;
                        instance.MapSession(session);
                    }
                }

                var destination = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Qml.Net.Tests", "Aot", "Generated"));
                var destinationNet = Path.Combine(destination, "net");
                var destinationNative = Path.Combine(destination, "native");
                
                session.WriteNativeCode(destinationNative);
            }
        }
    }
}
