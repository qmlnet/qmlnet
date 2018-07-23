using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PhotoFrame.Logic.UI.ViewModels
{
    public class ViewModelBorder : ViewModelBase
    {
        public ViewModelBorder(IFrameController frameController, IFrameConfig config)
            : base(frameController, config)
        {
            BorderColor = ColorToString(FrameController.GetNextBorderColor());
            BorderWidth = FrameConfig.BorderWidth;
        }

        public string BorderColor { get; }

        public uint BorderWidth { get; }

        private string ColorToString(Color color)
        {
            return $"#{color.R.ToString("x2")}{color.G.ToString("x2")}{color.B.ToString("x2")}";
        }
    }
}
