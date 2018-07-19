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
        [Signal("testSignal", NetVariantType.String)]
        public class TestQmlImport
        {
            public void TestMethod()
            {
                Console.WriteLine("Invoked method!");
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
