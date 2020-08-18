using System;
namespace Qml.Net
{
    public class QmlNetPaintHandler
    {
        private QmlNetPaintedItem m_paintedItem;

        protected QmlNetPaintedItem PaintedItem
        {
            get
            {
                return m_paintedItem;
            }
        }

        public QmlNetPaintHandler()
        {
        }

        public void __setPaintedItem(Int64 paintHandlerRef)
        {
            if (paintHandlerRef > 0)
            {
                m_paintedItem = new QmlNetPaintedItem((IntPtr)paintHandlerRef);
            }
            else
            {
                m_paintedItem = null;
            }
        }
    }
}
