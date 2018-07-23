using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PhotoFrame.Logic.BL
{
    public class CurrentPhotoChangedEventArgs : EventArgs { }
    public class TimerValueChangedEventArgs : EventArgs { }

    public class ColorizeInfo
    {
        public ColorizeInfo(double hue, double saturation, double lightness)
        {
            Hue = hue;
            Saturation = saturation;
            Lightness = lightness;
        }

        public double Hue { get; set; }
        public double Saturation { get; set; }
        public double Lightness { get; set; }
    }

    public interface IFrameController
    {
        event EventHandler<CurrentPhotoChangedEventArgs> CurrentPhotoChanged;
        event EventHandler<TimerValueChangedEventArgs> TimerValueChanged;

        Color GetNextBorderColor();
        ColorizeInfo GetNextColorizeInfo();

        ViewSwitchType GetNextViewSwitchType();
        string CurrentPhoto { get; }
        int TimerValue { get; }
        ViewType GetNextViewType();

        void Start();
        void Stop();
    }
}
