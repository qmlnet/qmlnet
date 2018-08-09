using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;
using PhotoFrame.Logic.UI.ViewModels;
using PhotoFrame.Logic.UI.Views;
using System;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Linq;

namespace PhotoFrame.Logic
{
    public class AppModel : IAppModel, INotifyPropertyChanged
    {
        public static UiDispatchDelegate UiDispatch { get; set; }

        private static AppModel _instance;
        public static AppModel Instance => _instance ?? (_instance = new AppModel());

        public static bool HasInstance => _instance != null;

        public event PropertyChangedEventHandler PropertyChanged;

        private ViewSwitchInfo _currentViewSwitchInfo;
        
        // ReSharper disable once MemberCanBePrivate.Global
        public ViewSwitchInfo CurrentViewSwitchInfo
        {
            // ReSharper disable once UnusedMember.Global
            get => _currentViewSwitchInfo;
            private set
            {
                if (Equals(_currentViewSwitchInfo, value)) return;
                _currentViewSwitchInfo = value;
                RaiseNotifyPropertyChanged();
                RaiseNotifyPropertyChanged(nameof(CurrentViewName));
                RaiseNotifyPropertyChanged(nameof(CurrentSwitchTypeName));
            }
        }

        // ReSharper disable once MemberCanBePrivate.Global
        public string CurrentViewName => _currentViewSwitchInfo?.ViewResourceId.Split('/').LastOrDefault()?.Split(new[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault()?.Replace(".qml", "");

        // ReSharper disable once MemberCanBePrivate.Global
        public string CurrentSwitchTypeName => _currentViewSwitchInfo?.SwitchType.ToString();

        private int _animationDurationMs = 500;
        // ReSharper disable once UnusedMember.Global
        public int AnimationDurationMs
        {
            get => _animationDurationMs;
            set
            {
                if (_animationDurationMs == value) return;
                _animationDurationMs = value;
                RaiseNotifyPropertyChanged();
            }
        }

        private int _timerValue;
        // ReSharper disable once MemberCanBePrivate.Global
        public int TimerValue
        {
            // ReSharper disable once UnusedMember.Global
            get => _timerValue;
            set
            {
                if (value == _timerValue) return;
                _timerValue = value;
                RaiseNotifyPropertyChanged();
            }
        }

        private string _currentlyUsedMbString = "";
        public string CurrentlyUsedMbString
        {
            // ReSharper disable once UnusedMember.Global
            get => _currentlyUsedMbString;
            set
            {
                if (_currentlyUsedMbString == value) return;
                _currentlyUsedMbString = value;
                RaiseNotifyPropertyChanged();
            }
        }

        private bool _showDebugInfo;
        // ReSharper disable once MemberCanBePrivate.Global
        public bool ShowDebugInfo
        {
            // ReSharper disable once UnusedMember.Global
            get => _showDebugInfo;
            set
            {
                if (_showDebugInfo == value) return;
                _showDebugInfo = value;
                RaiseNotifyPropertyChanged();
            }
        }

        private IFrameController _frameController;
        private IViewManager _viewManager;

        private IView _currentView;
        private readonly IFrameConfig _frameConfig;

        // ReSharper disable once MemberCanBePrivate.Global
        public AppModel()
        {
            if(_instance != null)
            {
                _currentViewSwitchInfo = _instance._currentViewSwitchInfo;
            }
            _instance = this;
            var wd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            if(wd == null)
                throw new NullReferenceException();
            _frameConfig = new FrameJsonConfig(Path.Combine(wd, "PhotoFrameConfig.json"));
            _frameConfig.FrameConfigChanged += (s, e) =>
            {
                InitAppWithConfig(_frameConfig);
            };
            InitAppWithConfig(_frameConfig);
        }

        private void InitAppWithConfig(IFrameConfig frameConfig)
        {
            if(_frameController != null)
            {
                _frameController.Stop();
            }
            _frameController = new FrameController(frameConfig, a => UiDispatch.Invoke(a));
            _viewManager = new ViewManager(this, _frameController, frameConfig);

            ShowDebugInfo = _frameConfig.ShowDebugInfo;

            _frameController.CurrentPhotoChanged += (s, e) =>
            {
                CreateAndShowNewView();
            };

            CreateAndShowNewView();

            _frameController.TimerValueChanged += (s, e) =>
            {
                TimerValue = _frameController.TimerValue;
            };

            _frameController.Start();
        }

        public void SwitchToView(ViewSwitchInfo switchInfo)
        {
            CurrentViewSwitchInfo = switchInfo;
        }

        private void CreateAndShowNewView()
        {
            var viewType = _frameController.GetNextViewType();
            var switchType = _frameController.GetNextViewSwitchType();
            if(_currentView != null)
            {
                _currentView.Deactivate();
            }
            _currentView = _viewManager.CreateView(viewType);
            _currentView.Activate(switchType);
        }

        private void RaiseNotifyPropertyChanged([CallerMemberName]string memberName = "")
        {
            if(PropertyChanged != null)
            {
                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(memberName));
            }
        }
    }
}
