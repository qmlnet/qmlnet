using System;
using System.Runtime.InteropServices;
using System.Threading;
using Qml.Net.Internal;

namespace Qml.Net
{
    public class QApplication : QCoreApplication
    {
        public QApplication()
            : this(null)
        {
        }

        public QApplication(string[] args, int flags = 0)
            : base(1, args, flags)
        {
        }

        internal QApplication(IntPtr existingApp)
            : base(existingApp)
        {
        }
    }
}