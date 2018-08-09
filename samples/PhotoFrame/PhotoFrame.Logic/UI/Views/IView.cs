using PhotoFrame.Logic.BL;

namespace PhotoFrame.Logic.UI.Views
{
    public interface IView
    {
        void Activate(ViewSwitchType switchType);
        void Deactivate();
    }
}
