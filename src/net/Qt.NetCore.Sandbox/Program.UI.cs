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
            variant.DateTime = new DateTime(1988, 9, 3);
            Console.WriteLine(variant.DateTime);
            
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
