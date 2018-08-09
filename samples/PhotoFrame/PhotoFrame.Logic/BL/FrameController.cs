using PhotoFrame.Logic.Config;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;

namespace PhotoFrame.Logic.BL
{
    public class FrameController : IFrameController
    {
        private Random _Random;
        private Timer _PhotoSwitchTimer;
        private TimeSpan _TimerValue = TimeSpan.Zero;

        IFrameConfig _FrameConfig;

        private PhotoDatabase _PhotosDatabase;
        private UiDispatchDelegate _UiDispatchDelegate;

        public event EventHandler<CurrentPhotoChangedEventArgs> CurrentPhotoChanged;
        public event EventHandler<TimerValueChangedEventArgs> TimerValueChanged;

        public FrameController(IFrameConfig frameConfig, UiDispatchDelegate uiDispatchDelegate)
        {
            _FrameConfig = frameConfig;
            _UiDispatchDelegate = uiDispatchDelegate;

            _PhotosDatabase = new PhotoDatabase(_FrameConfig.PhotosPath);
            _PhotosDatabase.PhotosListChanged += (s, e) => 
            {
                string nextPhoto = CalculateNextPhoto();
                _UiDispatchDelegate.Invoke(() => 
                {
                    CurrentPhoto = nextPhoto;
                });
            };

            _Random = new Random(DateTime.Now.Millisecond);

            _CurrentPhoto = CalculateNextPhoto();

            _PhotoSwitchTimer = new Timer(
                new TimerCallback(o => 
                {
                    if (_TimerValue >= _FrameConfig.PhotoShowTime)
                    {
                        _TimerValue = TimeSpan.Zero;
                        CurrentPhoto = CalculateNextPhoto();
                        RaiseTimerValueChanged();
                    }
                    else
                    {
                        _TimerValue = _TimerValue.Add(TimeSpan.FromSeconds(1));
                        RaiseTimerValueChanged();
                    }
                }));
        }

        public void Start()
        {
            _TimerValue = TimeSpan.Zero;
            _PhotoSwitchTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public void Stop()
        {
            _PhotoSwitchTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private string _CurrentPhoto;
        public string CurrentPhoto
        {
            get
            {
                return _CurrentPhoto;
            }
            private set
            {
                //if(!string.Equals(_CurrentPhoto, value))
                //{
                    _CurrentPhoto = value;
                    RaiseCurrentPhotoChanged();
                //}
            }
        }

        public int TimerValue
        {
            get
            {
                return (int)Math.Round(_FrameConfig.PhotoShowTime.TotalSeconds - _TimerValue.TotalSeconds);
            }
        }

        private bool _IsInitialViewSwitchType = true;

        public Color GetNextBorderColor()
        {
            Random r = new Random(DateTime.Now.Millisecond);
            int colorIndex = r.Next(_FrameConfig.BorderColors.Count);
            var borderColor = _FrameConfig.BorderColors[colorIndex];
            return borderColor;
        }

        public ColorizeInfo GetNextColorizeInfo()
        {
            //TODO: currently only black and white
            return new ColorizeInfo(0.0d, 0.0d, 0.0d);
        }

        public ViewSwitchType GetNextViewSwitchType()
        {
            if(_IsInitialViewSwitchType)
            {
                _IsInitialViewSwitchType = false;
                return ViewSwitchType.None;
            }
            return GetRandomEnumValue<ViewSwitchType>(ViewSwitchType.None);
        }

        public ViewType GetNextViewType()
        {
            return GetRandomEnumValue<ViewType>();
        }

        private string CalculateNextPhoto()
        {
            var photos = _PhotosDatabase.PhotoFiles.Where(pe => !pe.HasBeenShownInThisSession);
            if(!photos.Any())
            {
                _PhotosDatabase.ResetSession();
                photos = _PhotosDatabase.PhotoFiles.Where(pe => !pe.HasBeenShownInThisSession);
            }
            int photoIndex = _Random.Next(photos.Count());
            Console.Out.WriteLine($"Next Photo. Got {photos.Count()} remaining photos. Choosing Index {photoIndex}");
            var photoEntry = photos.ElementAt(photoIndex);
            photoEntry.HasBeenShownInThisSession = true;
            return Utils.GetQmlRelativePath(photoEntry.FilePath);
        }

        private void RaiseCurrentPhotoChanged()
        {
            CurrentPhotoChanged?.Invoke(this, new CurrentPhotoChangedEventArgs());
        }

        private void RaiseTimerValueChanged()
        {
            TimerValueChanged?.Invoke(this, new TimerValueChangedEventArgs());
        }

        private TEnum GetRandomEnumValue<TEnum>(params TEnum[] valuesToExclude) where TEnum : struct
        {
            var enumNames = Enum.GetNames(typeof(TEnum)).AsEnumerable();
            enumNames = enumNames.Where(en => !valuesToExclude.Any(vte => vte.ToString() == en));
            int index = _Random.Next(0, enumNames.Count());
            Enum.TryParse<TEnum>(enumNames.ElementAt(index), out TEnum val);
            return val;
        }
    }
}
