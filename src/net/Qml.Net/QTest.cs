using System;
using System.Runtime.InteropServices;
using Qml.Net.Internal;

namespace Qml.Net
{
    public class QTest
    {
        public static void QWait(int ms)
        {
            Interop.QTest.QWait(ms);
        }

        public delegate bool WaitForCb();

        public static bool QWaitFor(WaitForCb cb, int ms)
        {
            var realCb = new QTestInterop.WaitForCb(() => cb() ? 1 : 0);
            var handle = GCHandle.Alloc(realCb);
            var result = Interop.QTest.QWaitFor(Marshal.GetFunctionPointerForDelegate(realCb), ms);
            handle.Free();
            return result == 1;
        }

        public static bool QWaitFor(WaitForCb cb, TimeSpan time)
        {
            return QWaitFor(cb, (int)time.TotalMilliseconds);
        }
    }

    internal class QTestInterop
    {
        [NativeSymbol(Entrypoint = "qtest_qwait")]
        public QWaitDel QWait { get; set; }

        public delegate byte QWaitDel(int ms);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate int WaitForCb();

        [NativeSymbol(Entrypoint = "qtest_qWaitFor")]
        public QWaitForDel QWaitFor { get; set; }

        public delegate int QWaitForDel(IntPtr cb, int ms);
    }
}