using BenchmarkDotNet.Attributes;
using Qml.Net.Internal.Qml;

namespace Qml.Net.Benchmarks
{
    [Config(typeof(Config))]
    public class QObjectMethodBenchmarks
    {
        private static QGuiApplication _guiApplication;
        private static QQmlApplicationEngine _qmlApplicationEngine;
        private static INetQObject _qObject;

        [GlobalSetup]
        public void GlobalSetup()
        {
            _guiApplication = new QGuiApplication(new[] { "-platform", "offscreen" });
            _qmlApplicationEngine = new QQmlApplicationEngine();
            NetTestHelper.RunQml(
                _qmlApplicationEngine,
                @"
                        import QtQuick 2.0
                        import tests 1.0
                        Item {{
                        }}");
            _qObject = Qt.BuildQObject("TestQObject*");
        }

        [GlobalCleanup]
        public void GlobalCleanup()
        {
            _qObject.Dispose();
            QCoreApplication.ProcessEvents(QEventLoop.ProcessEventsFlag.AllEvents);
            _qmlApplicationEngine.Dispose();
            _guiApplication.Dispose();
        }

        [Benchmark]
        public void TestSlotInt()
        {
            _qObject.InvokeMethod("testSlotInt", 3);
        }
        
        [Benchmark]
        public void TestSlotQObject()
        {
            _qObject.InvokeMethod("testSlotQObject", _qObject);
        }
    }
}