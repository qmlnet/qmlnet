using System;
using System.Collections.Generic;
using System.Text;

namespace Qt.NetCore
{
    public interface IUiContext
    {
        /// <summary>
        /// Invokes the given Action on the UI thread
        /// </summary>
        /// <param name="action">Action to invoke on the UI thread</param>
        void InvokeOnGuiThread(Action action);
    }
}
