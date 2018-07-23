using PhotoFrame.Logic.BL;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoFrame.Logic.UI.ViewModels
{
    public class ViewSwitchInfo
    {
        public ViewSwitchInfo(string viewResourceId, IViewModel viewModel, ViewSwitchType switchType)
        {
            ViewResourceId = viewResourceId;
            ViewModel = viewModel;
            SwitchType = switchType;
        }

        public string ViewResourceId { get; private set; }
        public IViewModel ViewModel { get; private set; }
        public ViewSwitchType SwitchType { get; private set; }
        public string SwitchTypeString => SwitchType.ToString();
    }
}
