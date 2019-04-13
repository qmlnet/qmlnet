using System;

namespace Qml.Net
{
    public class QmlNetConfig
    {
        public static string QtBuildVersion => "qt-5.12.2-877b810";

        public static bool ListenForExceptionsWhenInvokingTasks { get; set; }

        public static event Action<AggregateException> UnhandledTaskException;

        internal static void RaiseUnhandledTaskException(AggregateException ex)
        {
            var handler = UnhandledTaskException;
            handler?.Invoke(ex);
        }

        public static bool ShouldEnsureUIThread { get; set; } = true;

        public static Action EnsureUIThreadDelegate = () =>
        {
            if (!QCoreApplication.IsMainThread)
            {
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