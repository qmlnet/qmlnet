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
        public class TestQmlImport
        {
            public TestObject GetObject()
            {
                return new TestObject
                {
                    Prop1 = "test1",
                    Prop2 = 3
                };
            }
        }

        public class TestObject
        {
            public string Prop1 { get; set; }
            
            public int Prop2 { get; set; }
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
