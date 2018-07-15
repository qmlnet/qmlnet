using Qt.NetCore.Qml;

namespace Qt.NetCore.Sandbox
{
    class Program
    {
        static void Main()
        {
            using (new QGuiApplication())
            {
                using (new QQmlApplicationEngine())
                {
                    
                }
            }
        }
    }
}
