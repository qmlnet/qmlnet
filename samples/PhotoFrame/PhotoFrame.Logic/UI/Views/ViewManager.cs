using System;
using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;

namespace PhotoFrame.Logic.UI.Views
{
    public class ViewManager : IViewManager
    {
        private readonly IAppModel _appModel;
        private readonly IFrameController _frameController;
        private readonly IFrameConfig _frameConfig;

        public ViewManager(IAppModel appModel, IFrameController frameController, IFrameConfig frameConfig)
        {
            _appModel = appModel;
            _frameController = frameController;
            _frameConfig = frameConfig;
        }

        public IView CreateView(ViewType viewType)
        {
            switch(viewType)
            {
                case ViewType.Normal:
                    return new ViewNormal(_appModel, _frameController, _frameConfig);
                case ViewType.Border:
                    return new ViewBorder(_appModel, _frameController, _frameConfig);
                case ViewType.Colorized:
                    return new ViewColorize(_appModel, _frameController, _frameConfig);
                default:
                    throw new NotSupportedException($"ViewType [{viewType}] not supported!");
            }
        }
    }
}
