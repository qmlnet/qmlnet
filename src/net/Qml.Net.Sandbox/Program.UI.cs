using System;
using System.IO;
using Qml.Net;

namespace Qml.Net.Sandbox
{
    class Program
    {
        [Signal("testSignal", NetVariantType.String)]
        public class TestQmlImport
        {
            
        }

        static int Main(string[] args)
        {
            using (var app = new QGuiApplication(args))
            {
                using (var engine = new QQmlApplicationEngine())
                {
                    engine.AddImportPath(Path.Combine(Directory.GetCurrentDirectory(), "Qml"));
                    
                    QQmlApplicationEngine.RegisterType<TestQmlImport>("test");

                    engine.Load("main.qml");

                    return app.Exec();
                }
            }
        }
    }
}
