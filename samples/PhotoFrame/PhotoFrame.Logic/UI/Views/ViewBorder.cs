using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;
using PhotoFrame.Logic.UI.ViewModels;

namespace PhotoFrame.Logic.UI.Views
{
    public class ViewBorder : ViewBase
    {
        public ViewBorder(IAppModel appModel, IFrameController frameController, IFrameConfig config) 
            : base("ViewBorder.qml", appModel, frameController, config)
        {
        }

        protected override IViewModel CreateViewModel()
        {
            return new ViewModelBorder(FrameController, FrameConfig);
        }

    }
}
