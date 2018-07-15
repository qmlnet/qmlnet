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
            using (var app = new QGuiApplication())
            {
                using (var engine = new QQmlApplicationEngine())
                {
                    QQmlApplicationEngine.RegisterType<TestQmlImport>();
                    
                    engine.Load("main.qml");

                    return app.Exec();
                }
            }
        }
    }
}
