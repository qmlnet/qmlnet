using PhotoFrame.Logic.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoFrame.Logic
{
    public interface IAppModel
    {
        void SwitchToView(ViewSwitchInfo switchInfo);
    }
}
