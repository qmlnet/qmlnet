using System;
using System.IO;
using Qml.Net.Aot;
using Qml.Net.Tests.Aot;

namespace Qml.Net.Tests.AotGenerate
{
    class Program
    {
        static void Main(string[] args)
        {
            var destination = Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), "..", "Qml.Net.Tests", "Aot", "Generated"));
            foreach (var file in Directory.GetFiles(destination))
            {
                File.Delete(file);
            }
            foreach (var directory in Directory.GetDirectories(destination))
            {
                Directory.Delete(directory, true);
            }
            
            using (var session = new AotSession(new AotSessionOptions
            {
                Name = "TestInterop",
                NetNamespace = "TestInterop"
            }))
            {
                foreach (var type in typeof(Aot.AotTestsBase).Assembly.GetTypes())
                {
                    if (typeof(IAotSessionMapper).IsAssignableFrom(type) && type.IsClass)
                    {
                        var instance = Activator.CreateInstance(type) as IAotSessionMapper;
                        instance?.MapSession(session);
                    }
                }

                var destinationNet = Path.Combine(destination, "net");
                var destinationNative = Path.Combine(destination, "native");
                
                session.WriteNativeCode(destinationNative);
                session.WriteNetCode(destinationNet);
            }
        }
    }
}
