using System;
using System.Drawing;

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

        public double Hue { get; }
        public double Saturation { get; }
        public double Lightness { get; }
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
