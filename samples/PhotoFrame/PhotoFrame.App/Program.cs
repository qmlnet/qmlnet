using PhotoFrame.Logic;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Qml.Net;
using System.IO;

namespace PhotoFrame.App
{
    internal static class Program
    {
        private static Timer _checkTimer;

        private static QGuiApplication _app;

        private static int Main(string[] args)
        {
            _checkTimer = new Timer((e) => 
            {
                CheckAndPrint();
            });
            _checkTimer.Change(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5));

            using (var app = new QGuiApplication(args))
            {
                _app = app;
                AppModel.UiDispatch += (a) => _app.Dispatch(a);
                QQmlApplicationEngine.ActivateMVVMBehavior();
                using (var engine = new QQmlApplicationEngine())
                {
                    QQmlApplicationEngine.RegisterType<AppModel>("app", 1, 1);

                    var assemblyDir = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().Location).LocalPath);
                    var mainQmlPath = Path.Combine(assemblyDir, "UI", "QML", "main.qml");
                    engine.Load(mainQmlPath);
                    int result = app.Exec();
                    _app = null;
                    return result;
                }
            }
        }

        private static void CheckAndPrint()
        {
            if(!AppModel.HasInstance)
            {
                return;
            }
            using (var proc = Process.GetCurrentProcess())
            {
                GC.Collect(2, GCCollectionMode.Forced, true);
                var memBytes = proc.WorkingSet64;
                var memKb = memBytes / 1024d;
                var memMb = memKb / 1024;
                var usedMbString = $"{memMb:0.00} MB";
                Console.WriteLine($"Used memory: {usedMbString}");
                _app?.Dispatch(() => 
                {
                    AppModel.Instance.CurrentlyUsedMbString = usedMbString;
                });
                
            }
        }
    }
}
