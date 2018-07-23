using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;

namespace PhotoFrame.Logic.UI.ViewModels
{
    public abstract class ViewModelBase : IViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected IFrameController FrameController { get; }
        protected IFrameConfig FrameConfig;
        
        public bool IsStopped { get; private set; }

        private string _ImageUri = "";
        public string ImageUri
        {
            get
            {
                return _ImageUri;
            }
            private set
            {
                if(!string.Equals(_ImageUri, value))
                {
                    _ImageUri = value;
                    RaisePropertyChanged();
                }
            }
        }

        public void Stop()
        {
            IsStopped = true;
            FrameController.CurrentPhotoChanged -= FrameController_CurrentPhotoChanged;
        }

        private static int _Counter = 0;

        public ViewModelBase(IFrameController frameController, IFrameConfig config)
        {
            FrameController = frameController;
            FrameConfig = config;
            ImageUri = frameController.CurrentPhoto;
            FrameController.CurrentPhotoChanged += FrameController_CurrentPhotoChanged;
            _Counter++;
            Console.WriteLine($"Number of ViewModels: {_Counter}");
        }

        private void FrameController_CurrentPhotoChanged(object sender, CurrentPhotoChangedEventArgs e)
        {
            if (!IsStopped)
            {
                ImageUri = FrameController.CurrentPhoto;
            }
        }

        ~ViewModelBase()
        {
            _Counter--;
            Console.WriteLine($"Number of ViewModels: {_Counter}");
        }

        protected void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
