using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;

namespace Qt.NetCore.Sandbox
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

        public void TestMethod()
        {

        }
    }
    
    class Program
    {
        static int Main(string[] args)
        {
            var path = System.Environment.GetEnvironmentVariable("PATH");
            path += ";" + @"D:\Git\Github\pauldotknopf\net-core-qml\src\native\build-QtNetCoreQml-Desktop_Qt_5_9_1_MSVC2017_64bit-Debug\debug";
            System.Environment.SetEnvironmentVariable("PATH", path);

            Task.Factory.StartNew(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000);
                    GC.Collect(GC.MaxGeneration);
                }
            });

            using (var r = new StringVector(0))
            {
                using (var app = new QGuiApplication(r))
                {
                    using (var engine = new QQmlApplicationEngine())
                    {
                        QQmlApplicationEngine.RegisterType<TestQmlImport>("test", 1, 1);

                        var method = NetTypeInfoManager.GetTypeInfo<TestQmlImport>().GetMethod(0);
                        Console.WriteLine(method.GetMethodName());
                        
                        NetTestHelper.RunMethod(engine,
                            @"
                                import QtQuick 2.0
                                import test 1.1

                                TestQmlImport {
                                }
                            ",
                            method,
                            null,
                            null);
                        
                        //engine.loadFile("main.qml");
                        //return app.exec();
                    }
                }
            }

            return 0;
        }
    }
}
