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
        private bool _testPropertyBool;
        private bool _testPropertyBool2;
        private string _testPropertyString = "$sdfsdfsdfsfsdfsdf€sdf£sfsdf";

        public TestQmlImport()
        {
            Console.WriteLine("Ctor");
            TestPropertyBool = true;
            TestPropertyBool2 = false;
        }

        ~TestQmlImport()
        {
            Console.WriteLine("~Ctor");
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

        public string TestPropertyString
        {
            get { return _testPropertyString; }
            set { _testPropertyString = value; }
        }

        public void TestMethod()
        {
            
        }

        public int TestMethodReturnInt()
        {
            return 15;
        }

        public int TestMethodReturnIntParamInt(int param)
        {
            return param;
        }

        public AnotherType TestObject()
        {
            return new AnotherType();
        }

        public string TestMethodString(string param)
        {
            return null;
        }
    }
    
    class Program
    {
        static int Main(string[] args)
        {
            var path = System.Environment.GetEnvironmentVariable("PATH");
            path += ";" + @"D:\Git\Github\pauldotknopf\net-core-qml\src\native\build-QtNetCoreQml-Desktop_Qt_5_9_1_MSVC2017_64bit-Debug\debug";
            System.Environment.SetEnvironmentVariable("PATH", path);

            var netVariant = new NetVariant();
            netVariant.SetString("");
            Console.WriteLine(netVariant.GetVariantType());
            netVariant.SetString(null);
            Console.WriteLine(netVariant.GetVariantType());
            
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
