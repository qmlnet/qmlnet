using PhotoFrame.Logic.BL;

namespace PhotoFrame.Logic.UI.Views
{
    public interface IViewManager
    {
        IView CreateView(ViewType viewType);
    }
}
