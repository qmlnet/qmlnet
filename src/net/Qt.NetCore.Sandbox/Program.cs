using System;
using System.Runtime.InteropServices;
using System.Security.Policy;

namespace Qt.NetCore.Sandbox
{
    public class TestQmlImport
    {
        private bool _testPropertyBool;
        private bool _testPropertyBool2;

        public TestQmlImport()
        {
            TestPropertyBool = true;
            TestPropertyBool2 = false;
        }

        public bool TestPropertyBool
        {
            get { return _testPropertyBool; }
            set { _testPropertyBool = value; }
        }

        public bool TestPropertyBool2
        {
            get { return _testPropertyBool2; }
            set { _testPropertyBool2 = value; }
        }

        public void TestMethod()
        {
            
        }

        public bool TestMethodReturnBool()
        {
            return false;
        }
    }

    class Test
    {
        public string TT { get; set; }
    }

    class Program
    {
        static int Main(string[] args)
        {
            var tt = new Test {TT = "sdfsd"};

            var handle = GCHandle.Alloc(tt);

            var ptr = GCHandle.ToIntPtr(handle);
            



            var path = System.Environment.GetEnvironmentVariable("PATH");
            path += ";" + @"D:\Git\Github\pauldotknopf\net-core-qml\src\native\build-QtNetCoreQml-Desktop_Qt_5_9_1_MSVC2017_64bit-Debug\debug";
            System.Environment.SetEnvironmentVariable("PATH", path);
         
            Initializer.Initialize();

            QCoreApplication.setAttribute(ApplicationAttribute.AA_EnableHighDpiScaling);
            
            using (var r = new StringVector(0))
            {
                using (var app = new QGuiApplication(r))
                {
                    using (var engine = new QQmlApplicationEngine())
                    {
                        //while (true)
                        {
                            Console.WriteLine(QtNetCoreQml.registerNetType(typeof(TestQmlImport).FullName + ", " + typeof(TestQmlImport).Assembly.FullName, "test", 1, 1, "TestQmlImport"));
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
