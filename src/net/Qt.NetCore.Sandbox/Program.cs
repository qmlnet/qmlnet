namespace Qt.NetCore.Sandbox
{
    class Program
    {
        static int Main(string[] args)
        {
            var path = System.Environment.GetEnvironmentVariable("PATH");
            path += ";" + @"D:\Git\Github\pauldotknopf\net-core-qml\src\native\build-QtNetCoreQml-Visual_Studio_64bit-Debug\debug";
            System.Environment.SetEnvironmentVariable("PATH", path);
            
            QCoreApplication.setAttribute(ApplicationAttribute.AA_EnableHighDpiScaling);

            using (var r = new StringVector(0))
            {
                using (var app = new QGuiApplication(r))
                {
                    using (var engine = new QQmlApplicationEngine())
                    {
                        engine.loadFile("main.qml");
                        return app.exec();
                    }
                }
            }
        }
    }
}
