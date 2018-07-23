using PhotoFrame.Logic.BL;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoFrame.Logic.UI.Views
{
    public interface IView
    {
        void Activate(ViewSwitchType switchType);
        void Deactivate();
    }
}
