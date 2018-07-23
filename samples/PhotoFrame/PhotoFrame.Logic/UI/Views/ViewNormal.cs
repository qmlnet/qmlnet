using System;
using System.Collections.Generic;
using System.Text;
using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;
using PhotoFrame.Logic.UI.ViewModels;

namespace PhotoFrame.Logic.UI.Views
{
    public class ViewNormal : ViewBase
    {
        public ViewNormal(IAppModel appModel, IFrameController frameController, IFrameConfig config) 
            : base("ViewNormal.qml", appModel, frameController, config)
        {
        }

        protected override IViewModel CreateViewModel()
        {
            return new ViewModelNormal(FrameController, FrameConfig);
        }
    }
}
