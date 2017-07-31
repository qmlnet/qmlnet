using System;
using System.Runtime.InteropServices;

namespace Qt.NetCore.Sandbox
{
    public class TestQmlImport
    {
        public void TestMethod()
        {
            
        }
    }

    class Program
    {
        static int Main(string[] args)
        {
            var path = System.Environment.GetEnvironmentVariable("PATH");
            path += ";" + @"D:\Git\Github\pauldotknopf\net-core-qml\src\native\build-QtNetCoreQml-Visual_Studio_64bit-Debug\debug";
            System.Environment.SetEnvironmentVariable("PATH", path);
            
            QCoreApplication.setAttribute(ApplicationAttribute.AA_EnableHighDpiScaling);
            
            NetInvoker.setGetMethodInfo();
            
            using (var r = new StringVector(0))
            {
                using (var app = new QGuiApplication(r))
                {
                    using (var engine = new QQmlApplicationEngine())
                    {
                        while (true)
                        {
                            Console.WriteLine(QtNetCoreQml.registerNetType(typeof(TestQmlImport).FullName, "test", 1, 1, "TestQmlImport"));
                            GC.Collect(GC.MaxGeneration);
                        }
                        //Console.WriteLine(QtNetCoreQml.registerNetType(typeof(TestQmlImport).FullName, "test", 1, 1, "TestQmlImport"));

                        engine.loadFile("main.qml");
                        return app.exec();
                    }
                }
            }
        }
    }
}
