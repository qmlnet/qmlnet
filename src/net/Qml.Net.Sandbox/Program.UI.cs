using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Qml.Net;
using Qml.Net.Internal.Qml;

namespace Qml.Net.Sandbox
{
    class Program
    {
        [Signal("testSignal", NetVariantType.String)]
        public class TestQmlImport
        {
            public void Test(INetJsValue jsValue)
            {
                Console.WriteLine("In net method, invoking callback");
                jsValue.Call();
            }

            public void AnotherMethod()
            {
                Console.WriteLine("Another method");
            }
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
