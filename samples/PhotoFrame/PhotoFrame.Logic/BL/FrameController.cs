using PhotoFrame.Logic.Config;
using System;
using System.Linq;
using System.Threading;
using System.Drawing;

namespace PhotoFrame.Logic.BL
{
    public class FrameController : IFrameController
    {
        private readonly Random _random;
        private readonly Timer _photoSwitchTimer;
        private TimeSpan _timerValue = TimeSpan.Zero;

        readonly IFrameConfig _frameConfig;

        private readonly PhotoDatabase _photosDatabase;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly UiDispatchDelegate _uiDispatchDelegate;

        public event EventHandler<CurrentPhotoChangedEventArgs> CurrentPhotoChanged;
        public event EventHandler<TimerValueChangedEventArgs> TimerValueChanged;

        public FrameController(IFrameConfig frameConfig, UiDispatchDelegate uiDispatchDelegate)
        {
            _frameConfig = frameConfig;
            _uiDispatchDelegate = uiDispatchDelegate;

            _photosDatabase = new PhotoDatabase(_frameConfig.PhotosPath);
            _photosDatabase.PhotosListChanged += (s, e) => 
            {
                string nextPhoto = CalculateNextPhoto();
                _uiDispatchDelegate.Invoke(() => 
                {
                    CurrentPhoto = nextPhoto;
                });
            };

            _random = new Random(DateTime.Now.Millisecond);

            _currentPhoto = CalculateNextPhoto();

            _photoSwitchTimer = new Timer(
                o => 
                {
                    if (_timerValue >= _frameConfig.PhotoShowTime)
                    {
                        _timerValue = TimeSpan.Zero;
                        CurrentPhoto = CalculateNextPhoto();
                        RaiseTimerValueChanged();
                    }
                    else
                    {
                        _timerValue = _timerValue.Add(TimeSpan.FromSeconds(1));
                        RaiseTimerValueChanged();
                    }
                });
        }

        public void Start()
        {
            _timerValue = TimeSpan.Zero;
            _photoSwitchTimer.Change(TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        public void Stop()
        {
            _photoSwitchTimer.Change(Timeout.Infinite, Timeout.Infinite);
        }

        private string _currentPhoto;
        public string CurrentPhoto
        {
            get => _currentPhoto;
            private set
            {
                //if(!string.Equals(_CurrentPhoto, value))
                //{
                    _currentPhoto = value;
                    RaiseCurrentPhotoChanged();
                //}
            }
        }

        public int TimerValue => (int)Math.Round(_frameConfig.PhotoShowTime.TotalSeconds - _timerValue.TotalSeconds);

        private bool _isInitialViewSwitchType = true;

        public Color GetNextBorderColor()
        {
            Random r = new Random(DateTime.Now.Millisecond);
            int colorIndex = r.Next(_frameConfig.BorderColors.Count);
            var borderColor = _frameConfig.BorderColors[colorIndex];
            return borderColor;
        }

        public ColorizeInfo GetNextColorizeInfo()
        {
            //TODO: currently only black and white
            return new ColorizeInfo(0.0d, 0.0d, 0.0d);
        }

        public ViewSwitchType GetNextViewSwitchType()
        {
            if (!_isInitialViewSwitchType) return GetRandomEnumValue(ViewSwitchType.None);
            _isInitialViewSwitchType = false;
            return ViewSwitchType.None;
        }

        public ViewType GetNextViewType()
        {
            return GetRandomEnumValue<ViewType>();
        }

        private string CalculateNextPhoto()
        {
            var photos = _photosDatabase.PhotoFiles.Where(pe => !pe.HasBeenShownInThisSession);
            var photoEntries = photos.ToList();
            if(!photoEntries.Any())
            {
                _photosDatabase.ResetSession();
                 photoEntries = _photosDatabase.PhotoFiles.Where(pe => !pe.HasBeenShownInThisSession).ToList();
            }
            var photoIndex = _random.Next(photoEntries.Count());
            Console.Out.WriteLine($"Next Photo. Got {photoEntries.Count()} remaining photos. Choosing Index {photoIndex}");
            var photoEntry = photoEntries.ElementAt(photoIndex);
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
            enumNames = enumNames.Where(en => valuesToExclude.All(vte => vte.ToString() != en));
            var enumerableNames = enumNames.ToList();
            var index = _random.Next(0, enumerableNames.Count);
            Enum.TryParse(enumerableNames.ElementAt(index), out TEnum val);
            return val;
        }
    }
}
