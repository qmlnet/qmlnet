using Moq;
using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;
using PhotoFrame.Logic.UI.ViewModels;
using PhotoFrame.Logic.UI.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PhotoFrame.Logic.Tests.UI.Views
{
    public abstract class ViewTestBase<TView> where TView : ViewBase
    {
        protected IAppModel _AppModelMock;
        protected IFrameController _FrameControllerMock;
        protected IFrameConfig _FrameConfigMock;

        public ViewTestBase()
        {
            _AppModelMock = Mock.Of<IAppModel>();
            _FrameControllerMock = Mock.Of<IFrameController>();
            _FrameConfigMock = Mock.Of<IFrameConfig>();
        }

        protected abstract TView CreateUUT();
        protected abstract string ExpectedUri { get; }

        [Fact]
        public void ActivationWorks()
        {
            var uut = CreateUUT();

            var appModelMock = Mock.Get(_AppModelMock);
            appModelMock.Setup(a => a.SwitchToView(It.IsAny<ViewSwitchInfo>()));

            uut.Activate(ViewSwitchType.Fade);

            appModelMock.Verify(a => a.SwitchToView(It.Is<ViewSwitchInfo>(vsi => vsi.SwitchType == ViewSwitchType.Fade)));
            appModelMock.Verify(a => a.SwitchToView(It.Is<ViewSwitchInfo>(vsi => object.ReferenceEquals(vsi.ViewModel, uut.ViewModel))));
            appModelMock.Verify(a => a.SwitchToView(It.Is<ViewSwitchInfo>(vsi => string.Equals(vsi.ViewResourceId, ExpectedUri))));
        }
    }
}
