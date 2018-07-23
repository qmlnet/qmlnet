using PhotoFrame.Logic.BL;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoFrame.Logic.UI.Views
{
    public interface IViewManager
    {
        IView CreateView(ViewType viewType);
    }
}
