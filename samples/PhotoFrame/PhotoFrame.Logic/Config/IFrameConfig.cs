using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PhotoFrame.Logic.Config
{
    public class FrameConfigChangedEventArgs {}

    public interface IFrameConfig
    {
        event EventHandler<FrameConfigChangedEventArgs> FrameConfigChanged;

        List<Color> BorderColors { get; }
        uint BorderWidth { get; }

        TimeSpan PhotoShowTime { get; }
        string PhotosPath { get; }

        bool ShowDebugInfo { get; }
    }
}
