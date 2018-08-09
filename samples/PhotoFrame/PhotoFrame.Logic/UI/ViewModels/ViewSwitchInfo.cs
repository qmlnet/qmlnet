using PhotoFrame.Logic.BL;

namespace PhotoFrame.Logic.UI.ViewModels
{
    public class ViewSwitchInfo
    {
        public ViewSwitchInfo(string viewResourceId, IViewModel viewModel, ViewSwitchType switchType)
        {
            ViewResourceId = viewResourceId;
            ViewModel = viewModel;
            SwitchType = switchType;
        }

        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public string ViewResourceId { get; private set; }
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public IViewModel ViewModel { get; private set; }
        // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Local
        public ViewSwitchType SwitchType { get; private set; }
        // ReSharper disable once UnusedMember.Global
        public string SwitchTypeString => SwitchType.ToString();
    }
}
