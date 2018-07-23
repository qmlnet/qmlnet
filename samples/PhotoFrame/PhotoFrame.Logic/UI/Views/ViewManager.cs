using System;
using System.Collections.Generic;
using System.Text;
using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;

namespace PhotoFrame.Logic.UI.Views
{
    public class ViewManager : IViewManager
    {
        private IAppModel _AppModel;
        private IFrameController _FrameController;
        private IFrameConfig _FrameConfig;

        public ViewManager(IAppModel appModel, IFrameController frameController, IFrameConfig frameConfig)
        {
            _AppModel = appModel;
            _FrameController = frameController;
            _FrameConfig = frameConfig;
        }

        public IView CreateView(ViewType viewType)
        {
            switch(viewType)
            {
                case ViewType.Normal:
                    return new ViewNormal(_AppModel, _FrameController, _FrameConfig);
                case ViewType.Border:
                    return new ViewBorder(_AppModel, _FrameController, _FrameConfig);
                case ViewType.Colorized:
                    return new ViewColorize(_AppModel, _FrameController, _FrameConfig);
                default:
                    throw new NotSupportedException($"ViewType [{viewType}] not supported!");
            }
        }
    }
}
