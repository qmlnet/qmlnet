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

        static int Main()
        {
            using (var app = new QGuiApplication())
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
