using System;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;
using Qt.NetCore.Qml;
using Qt.NetCore.Types;

namespace Qt.NetCore.Sandbox
{
    class Program
    {
        public class TestQmlImport
        {
            public AnotherType Create()
            {
                return new AnotherType();
            }
            
            public void TestMethod(AnotherType anotherType)
            {

            }
        }
        
        public class AnotherType
        {
            private static int _instanceCounter = 0;
            
            public AnotherType()
            {
                Console.WriteLine($"AnotherType:{Interlocked.Increment(ref _instanceCounter)}");
            }

            ~AnotherType()
            {
                Console.WriteLine($"AnotherType:{Interlocked.Decrement(ref _instanceCounter)}");
            }
        }
        
        static int Main()
        {
            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    GC.Collect(GC.MaxGeneration);
                }
            });
            using (var app = new QGuiApplication())
            {
                using (var engine = new QQmlApplicationEngine())
                {
                    var type = NetTypeManager.GetTypeInfo<TestQmlImport>();
                    
                    QQmlApplicationEngine.RegisterType<TestQmlImport>("test");
                    
                    engine.Load("main.qml");

                    return app.Exec();
                }
            }
        }
    }
}
