using System;

namespace Qml.Net
{
    public class QEventLoop
    {
        [Flags]
        public enum ProcessEventsFlag
        {
            AllEvents = 0x00,
            ExcludeUserInputEvents = 0x01,
            ExcludeSocketNotifiers = 0x02,
            WaitForMoreEvents = 0x04,
            X11ExcludeTimers = 0x08,
            EventLoopExec = 0x20,
            DialogExec = 0x40
        }
    }
}