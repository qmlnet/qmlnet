using System;
using Qt.NetCore.Qml;

namespace Qt.NetCore.Sandbox
{
    class Program
    {
        public class TestQmlImport
        {
            
        }
        
        static int Main()
        {
            var variant = new NetVariant();
            Console.WriteLine(variant.String);
            
            using (var app = new QGuiApplication())
            {
                using (var engine = new QQmlApplicationEngine())
                {
                    QQmlApplicationEngine.RegisterType<TestQmlImport>("test");
                    
                    engine.Load("main.qml");

                    return app.Exec();
                }
            }
        }
    }
}
