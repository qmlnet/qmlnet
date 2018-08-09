using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;
using PhotoFrame.Logic.UI.ViewModels;

namespace PhotoFrame.Logic.UI.Views
{
    public class ViewColorize : ViewBase
    {
        public ViewColorize(IAppModel appModel, IFrameController frameController, IFrameConfig frameConfig) 
            : base("ViewColorized.qml", appModel, frameController, frameConfig)
        {
        }

        protected override IViewModel CreateViewModel()
        {
            return new ViewModelColorize(FrameController, FrameConfig);
        }
    }
}
