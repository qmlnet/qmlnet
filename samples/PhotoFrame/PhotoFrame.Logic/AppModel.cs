using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;
using PhotoFrame.Logic.UI.ViewModels;
using PhotoFrame.Logic.UI.Views;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Linq;

namespace PhotoFrame.Logic
{
    public class AppModel : IAppModel, INotifyPropertyChanged
    {
        public static UIDispatchDelegate UIDispatch { get; set; }

        private static AppModel _Instance = null;
        public static AppModel Instance {
            get
            {
                if(_Instance == null)
                {
                    _Instance = new AppModel();
                }
                return _Instance;
            }
        }

        public static bool HasInstance
        {
            get
            {
                return _Instance != null;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private ViewSwitchInfo _CurrentViewSwitchInfo;
        public ViewSwitchInfo CurrentViewSwitchInfo
        {
            get
            {
                return _CurrentViewSwitchInfo;
            }
            private set
            {
                if(!object.Equals(_CurrentViewSwitchInfo, value))
                {
                    _CurrentViewSwitchInfo = value;
                    RaiseNotifyPropertyChanged();
                    RaiseNotifyPropertyChanged("CurrentViewName");
                    RaiseNotifyPropertyChanged("CurrentSwitchTypeName");
                }
            }
        }

        public string CurrentViewName
        {
            get
            {
                return _CurrentViewSwitchInfo?.ViewResourceId.Split('/').LastOrDefault()?.Split(new string[] { "\\" }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault()?.Replace(".qml", "");
            }
        }

        public string CurrentSwitchTypeName
        {
            get
            {
                return _CurrentViewSwitchInfo?.SwitchType.ToString();
            }
        }

        private int _AnimationDurationMs = 500;
        public int AnimationDurationMs
        {
            get
            {
                return _AnimationDurationMs;
            }
            set
            {
                if(_AnimationDurationMs != value)
                {
                    _AnimationDurationMs = value;
                    RaiseNotifyPropertyChanged();
                }
            }
        }

        private int _TimerValue;
        public int TimerValue
        {
            get
            {
                return _TimerValue;
            }
            set
            {
                if(value != _TimerValue)
                {
                    _TimerValue = value;
                    RaiseNotifyPropertyChanged();
                }
            }
        }

        private string _CurrentlyUsedMBString = "";
        public string CurrentlyUsedMBString
        {
            get
            {
                return _CurrentlyUsedMBString;
            }
            set
            {
                if(_CurrentlyUsedMBString != value)
                {
                    _CurrentlyUsedMBString = value;
                    RaiseNotifyPropertyChanged();
                }
            }
        }

        private bool _ShowDebugInfo = false;
        public bool ShowDebugInfo
        {
            get
            {
                return _ShowDebugInfo;
            }
            set
            {
                if(_ShowDebugInfo != value)
                {
                    _ShowDebugInfo = value;
                    RaiseNotifyPropertyChanged();
                }
            }
        }

        private IFrameController _FrameController;
        private IViewManager _ViewManager;

        private IView _CurrentView = null;
        private IFrameConfig _FrameConfig;

        public AppModel()
        {
            if(_Instance != null)
            {
                _CurrentViewSwitchInfo = _Instance._CurrentViewSwitchInfo;
            }
            _Instance = this;
            string wd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            _FrameConfig = new FrameJsonConfig(Path.Combine(wd, "PhotoFrameConfig.json"));
            _FrameConfig.FrameConfigChanged += (s, e) =>
            {
                InitAppWithConfig(_FrameConfig);
            };
            InitAppWithConfig(_FrameConfig);
        }

        private void InitAppWithConfig(IFrameConfig frameConfig)
        {
            if(_FrameController != null)
            {
                _FrameController.Stop();
            }
            _FrameController = new FrameController(frameConfig, new UIDispatchDelegate((a) => UIDispatch.Invoke(a)));
            _ViewManager = new ViewManager(this, _FrameController, frameConfig);

            ShowDebugInfo = _FrameConfig.ShowDebugInfo;

            _FrameController.CurrentPhotoChanged += (s, e) =>
            {
                CreateAndShowNewView();
            };

            CreateAndShowNewView();

            _FrameController.TimerValueChanged += (s, e) =>
            {
                TimerValue = _FrameController.TimerValue;
            };

            _FrameController.Start();
        }

        public void SwitchToView(ViewSwitchInfo switchInfo)
        {
            CurrentViewSwitchInfo = switchInfo;
        }

        private void CreateAndShowNewView()
        {
            var viewType = _FrameController.GetNextViewType();
            var switchType = _FrameController.GetNextViewSwitchType();
            if(_CurrentView != null)
            {
                _CurrentView.Deactivate();
            }
            _CurrentView = _ViewManager.CreateView(viewType);
            _CurrentView.Activate(switchType);
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
