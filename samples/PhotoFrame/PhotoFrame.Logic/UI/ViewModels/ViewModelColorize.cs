using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoFrame.Logic.UI.ViewModels
{
    public class ViewModelColorize : ViewModelBase
    {
        public ViewModelColorize(IFrameController frameController, IFrameConfig config)
            : base(frameController, config)
        {
            var colorizeInfo = frameController.GetNextColorizeInfo();
            Hue = colorizeInfo.Hue;
            Saturation = colorizeInfo.Saturation;
            Lightness = colorizeInfo.Lightness;
        }

        public double Hue { get; }
        public double Saturation { get; }
        public double Lightness { get; }
    }
}
