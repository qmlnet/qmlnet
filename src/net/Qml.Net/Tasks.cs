using System;

namespace Qml.Net
{
    public static class Tasks
    {
        public static bool ListenForExceptionsWhenInvokingTasks { get; set; }

        public static event Action<AggregateException> UnhandledTaskException;

        internal static void RaiseUnhandledTaskException(AggregateException ex)
        {
            var handler = UnhandledTaskException;
            handler?.Invoke(ex);
        }
    }
}