using Moq;
using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PhotoFrame.Logic.Tests.UI.ViewModels
{
    public class ViewSwitchInfoTest
    {
        private IViewModel _ViewModelMock;

        public ViewSwitchInfoTest()
        {
            _ViewModelMock = Mock.Of<IViewModel>();
        }

        protected ViewSwitchInfo CreateUUT(string viewResourceId, ViewSwitchType viewSwitchType)
        {
            var result = new ViewSwitchInfo(viewResourceId, _ViewModelMock, viewSwitchType);

            return result;
        }

        [Fact]
        public void ViewResourceIdIsSetCorrectly()
        {
            var uut = CreateUUT("View resource id", ViewSwitchType.Horizontal);

            Assert.Equal("View resource id", uut.ViewResourceId);
        }

        [Fact]
        public void ViewModelIsSetCorrectly()
        {
            var uut = CreateUUT("View resource id", ViewSwitchType.Horizontal);

            Assert.Same(_ViewModelMock, uut.ViewModel);
        }

        [Fact]
        public void ViewSwitchTypeIsSetCorrectly()
        {
            var uut = CreateUUT("View resource id", ViewSwitchType.Horizontal);

            Assert.Equal(ViewSwitchType.Horizontal, uut.SwitchType);
        }
    }
}
