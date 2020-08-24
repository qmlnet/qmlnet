using System;
namespace Qml.Net
{
    public class QmlNetPaintHandler
    {
        private QmlNetRecordingPaintedItem m_paintedItem;

        protected QmlNetRecordingPaintedItem PaintedItem => m_paintedItem;

        public QmlNetPaintHandler()
        {
        }

        public void __setPaintedItem(Int64 paintHandlerRef, Int64 inetqpainterRef)
        {
            if (paintHandlerRef > 0)
            {
                m_paintedItem = new QmlNetRecordingPaintedItem((IntPtr)paintHandlerRef, (IntPtr)inetqpainterRef);
            }
            else
            {
                m_paintedItem = null;
            }
        }
    }
}
