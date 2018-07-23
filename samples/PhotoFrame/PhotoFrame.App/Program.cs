using PhotoFrame.Logic;
using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.UI.ViewModels;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Qml.Net;

namespace PhotoFrame.App
{
    class Program
    {
        static Timer _CheckTimer;

        private static QGuiApplication _App;

        static int Main(string[] args)
        {
            _CheckTimer = new Timer((e) => 
            {
                CheckAndPrint();
            });
            _CheckTimer.Change(TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(5));

            using (var app = new QGuiApplication(args))
            {
                _App = app;
                AppModel.UIDispatch += (a) => _App.Dispatch(a);
                QQmlApplicationEngine.ActivateMVVMBehavior();
                using (var engine = new QQmlApplicationEngine())
                {
                    QQmlApplicationEngine.RegisterType<AppModel>("app", 1, 1);

                    engine.Load("UI/QML/main.qml");
                    int result = app.Exec();
                    _App = null;
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
                GC.Collect();
                var memBytes = proc.WorkingSet64;
                var memKb = memBytes / 1024d;
                var memMB = memKb / 1024;
                var usedMBString = $"{memMB:0.00} MB";
                Console.WriteLine($"Used memory: {usedMBString}");
                _App?.Dispatch(() => 
                {
                    AppModel.Instance.CurrentlyUsedMBString = usedMBString;
                });
                
            }
        }
    }
}
