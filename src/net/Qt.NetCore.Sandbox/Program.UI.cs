using System;
using Qt.NetCore.Qml;
using Qt.NetCore.Types;

namespace Qt.NetCore.Sandbox
{
    class Program
    {
        public class TestQmlImport
        {
            
        }
        
        static int Main()
        {
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
