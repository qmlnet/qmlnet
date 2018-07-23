using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;
using PhotoFrame.Logic.UI.ViewModels;

namespace PhotoFrame.Logic.UI.Views
{
    public abstract class ViewBase : IView
    {
        private string _Uri;

        protected IAppModel AppModel { get; }
        protected IFrameController FrameController { get; }
        protected IFrameConfig FrameConfig { get; }

        public IViewModel ViewModel { get; private set; }

        public ViewBase(string uri, IAppModel appModel, IFrameController frameController, IFrameConfig frameConfig)
        {
            _Uri = uri;
            AppModel = appModel;
            FrameController = frameController;
            FrameConfig = frameConfig;
        }

        public void Activate(ViewSwitchType switchType)
        {
            ViewModel = CreateViewModel();
            AppModel.SwitchToView(new ViewSwitchInfo(_Uri, ViewModel, switchType));
        }

        public void Deactivate()
        {
            ViewModel.Stop();
            ViewModel = null;
        }

        protected abstract IViewModel CreateViewModel();
    }
}
