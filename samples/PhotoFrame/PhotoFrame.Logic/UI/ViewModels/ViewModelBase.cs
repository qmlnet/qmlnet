using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PhotoFrame.Logic.UI.ViewModels
{
    public abstract class ViewModelBase : IViewModel, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected IFrameController FrameController { get; }
        protected readonly IFrameConfig FrameConfig;
        
        // ReSharper disable once MemberCanBePrivate.Global
        public bool IsStopped { get; private set; }

        private string _imageUri = "";
        public string ImageUri
        {
            get => _imageUri;
            private set
            {
                if (string.Equals(_imageUri, value)) return;
                _imageUri = value;
                RaisePropertyChanged();
            }
        }

        public void Stop()
        {
            IsStopped = true;
            FrameController.CurrentPhotoChanged -= FrameController_CurrentPhotoChanged;
        }

        private static int _counter;

        protected ViewModelBase(IFrameController frameController, IFrameConfig config)
        {
            FrameController = frameController;
            FrameConfig = config;
            ImageUri = frameController.CurrentPhoto;
            FrameController.CurrentPhotoChanged += FrameController_CurrentPhotoChanged;
            _counter++;
            Console.WriteLine($"Number of ViewModels: {_counter}");
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
            _counter--;
            Console.WriteLine($"Number of ViewModels: {_counter}");
        }

        protected void RaisePropertyChanged([CallerMemberName]string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
