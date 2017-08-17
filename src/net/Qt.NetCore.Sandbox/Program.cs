using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Threading;

namespace Qt.NetCore.Sandbox
{
    public class TestQmlImport
    {
        private bool _testPropertyBool;
        private bool _testPropertyBool2;

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

        public void TestMethod()
        {
            
        }

        public bool TestMethodReturnInt()
        {
            return false;
        }

        public int TestMethodReturnIntParamInt(int param)
        {
            return param;
        }
    }
    
    class Program
    {
        static int Main(string[] args)
        {
            var path = System.Environment.GetEnvironmentVariable("PATH");
            path += ";" + @"D:\Git\Github\pauldotknopf\net-core-qml\src\native\build-QtNetCoreQml-Desktop_Qt_5_9_1_MSVC2017_64bit-Debug\debug";
            System.Environment.SetEnvironmentVariable("PATH", path);
         
            Initializer.Initialize();

            var typeInfo = NetTypeInfoManager.GetTypeInfo(typeof(TestQmlImport).FullName + ", " +
                                           typeof(TestQmlImport).Assembly.FullName);

            //var netVariant = new NetVariant();
            //Console.WriteLine(netVariant.GetVariantType());
            //netVariant.SetNetInstance(NetTypeInfoManager.CreateInstance(typeInfo));
            //Console.WriteLine(netVariant.GetVariantType());
            //netVariant.SetInt(0);
            ////netVariant.Dispose();

            //while (true)
            //{
            //    GC.Collect(GC.MaxGeneration);
            //}

            //return 0;

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
