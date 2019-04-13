using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using Qml.Net.Internal;
using Qml.Net.Internal.Qml;

namespace Qml.Net
{
    public class QGuiApplication : QCoreApplication
    {
        public QGuiApplication()
            : this(null)
        {
        }

        public QGuiApplication(string[] args, int flags = 0)
            : base(1, args, flags)
        {
        }

        internal QGuiApplication(IntPtr existingApp)
            : base(existingApp)
        {
        }
    }
}