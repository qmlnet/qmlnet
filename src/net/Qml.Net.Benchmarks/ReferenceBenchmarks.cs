using BenchmarkDotNet.Attributes;

namespace Qml.Net.Benchmarks
{
    [Config(typeof(Config))]
    public class ReferenceBenchmarks
    {
        private static QGuiApplication _guiApplication;
        private static QQmlApplicationEngine _qmlApplicationEngine;
        private static bool _initialized;

        [GlobalSetup]
        public void GlobalSetup()
        {
            if (!_initialized)
            {
                Qml.RegisterType<QmlType>("tests");
                _initialized = true;
            }
        }
        
        [IterationSetup]
        public void Setup()
        {
            _guiApplication = new QGuiApplication(new[]{"-platform", "offscreen"});
            _qmlApplicationEngine = new QQmlApplicationEngine();
        }

        [IterationCleanup]
        public void Cleanup()
        {
            _qmlApplicationEngine.Dispose();
            _guiApplication.Dispose();
        }
        
        [Benchmark]
        public void Run()
        {
            _qmlApplicationEngine.LoadData(@"
                import QtQuick 2.0
                import tests 1.0
    
                Item {
                    property int count: 0
                    Timer {
                        interval: 1
                        running: true
                        repeat: true
                        onTriggered: {
                            var o1 = test.GetObject()
                            var o2 = test.GetObject()
                            var o3 = test.GetObject()
                            var o4 = test.GetObject()
                            var i;
                            for (i = 0; i < 500; i++) { 
                                var t = test.GetObject()
                            }
                            count++
                            if(count == 100) {
                                test.TestFinished()
                            }
                        }
                    }
                    QmlType {
                        id: test
                    }
                }
            ");

            _guiApplication.Exec();
        }

        public class QmlType
        {
            private readonly InnerType _object = new InnerType();
            
            public object GetObject()
            {
                return _object;
            }

            public void TestFinished()
            {
                _guiApplication.Exit();
            }
            
            public class InnerType
            {
                
            }
        }
    }
}