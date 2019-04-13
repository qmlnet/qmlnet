using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Qml.Net;
using Qml.Net.Internal.Qml;

namespace Qml.Net.Sandbox
{
    class Program
    {
        public class TestQmlImport
        {
            public static TestObject ts = new TestObject();

            public TestObject GetObject()
            {
                return ts;
            }
        }

        public class TestObject
        {
            public void Method()
            {
            }
        }

        static int Main(string[] args)
        {
            using (var app = new QGuiApplication(args))
            {
                using (var engine = new QQmlApplicationEngine())
                {
                    engine.AddImportPath(Path.Combine(Directory.GetCurrentDirectory(), "Qml"));

                    Qml.RegisterType<TestQmlImport>("test");

                    engine.Load("main.qml");

                    return app.Exec();
                }
            }
        }
    }
}
