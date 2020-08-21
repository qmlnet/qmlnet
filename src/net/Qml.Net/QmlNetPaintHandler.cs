using System;
namespace Qml.Net
{
    public class QmlNetPaintHandler
    {
        private QmlNetPaintedItem m_paintedItem;

        protected QmlNetPaintedItem PaintedItem => m_paintedItem;

        public QmlNetPaintHandler()
        {
        }

        public void __setPaintedItem(Int64 paintHandlerRef, Int64 inetqpainterRef)
        {
            if (paintHandlerRef > 0)
            {
                m_paintedItem = new QmlNetPaintedItem((IntPtr)paintHandlerRef, (IntPtr)inetqpainterRef);
            }
            else
            {
                m_paintedItem = null;
            }
        }
    }
}
