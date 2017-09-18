using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Runners;

namespace Qt.NetCore.Sandbox
{
    class Program
    {
        public class AnotherType
        {
            public AnotherType()
            {
                Console.WriteLine("Ctor2");
            }

            ~AnotherType()
            {
                Console.WriteLine("~Ctor2");
            }

            public void Test()
            {
            }
        }

        public class TestQmlImport
        {
            public TestQmlImport()
            {
                Console.WriteLine("Ctor");
            }

            ~TestQmlImport()
            {
                Console.WriteLine("~Ctor");
            }

            public AnotherType Create()
            {
                return new AnotherType();
            }
            
            public void TestMethod(AnotherType anotherType)
            {

            }
        }

        static int Main()
        {
#if DEBUG
            Helpers.LoadDebugVariables();
#endif

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(10);
                    GC.Collect(GC.MaxGeneration);
                }
            // ReSharper disable FunctionNeverReturns
            });
            // ReSharper restore FunctionNeverReturns

            using (var r = new StringVector(0))
            {
                using (var app = new QGuiApplication(r))
                {
                    using (var engine = new QQmlApplicationEngine())
                    {
                        QQmlApplicationEngine.RegisterType<TestQmlImport>("test", 1, 1);
                        
                        engine.loadFile("main.qml");
                        return app.exec();
                    }
                }
            }
        }
    }
}
