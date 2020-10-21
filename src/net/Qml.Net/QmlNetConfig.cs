using System;

namespace Qml.Net
{
    public class QmlNetConfig
    {
        public static string QtBuildVersion => "qt-5.15.1-7fc8b10";

        public static bool ListenForExceptionsWhenInvokingTasks { get; set; }

        public static event Action<AggregateException> UnhandledTaskException;

        internal static void RaiseUnhandledTaskException(AggregateException ex)
        {
            var handler = UnhandledTaskException;
            handler?.Invoke(ex);
        }

        public static bool ShouldEnsureUIThread { get; set; } = true;

        public static bool AutoGenerateNotifySignals { get; set; } = false;
        
        public static Action EnsureUIThreadDelegate = () =>
        {
            if (!QCoreApplication.IsMainThread)
            {
                Console.WriteLine(@"You are using QML functionality outside the UI Thread!
Refer to https://github.com/qmlnet/qmlnet/issues/112 to get more information.
Stack Trace:
" + System.Environment.StackTrace);
                throw new Exception(
                    "You must be on the UI thread to perform this task. See https://github.com/qmlnet/qmlnet/issues/112");
            }
        };

        internal static void EnsureUIThread()
        {
            if (!ShouldEnsureUIThread)
            {
                return;
            }

            EnsureUIThreadDelegate?.Invoke();
        }
    }
}