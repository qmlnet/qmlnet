using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoFrame.Logic.UI.ViewModels
{
    public class ViewModelNormal : ViewModelBase
    {
        public ViewModelNormal(IFrameController frameController, IFrameConfig config)
            : base(frameController, config)
        {
        }
    }
}
