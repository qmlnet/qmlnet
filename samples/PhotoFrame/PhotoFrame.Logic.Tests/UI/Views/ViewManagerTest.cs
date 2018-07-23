using Moq;
using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;
using PhotoFrame.Logic.UI.Views;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Xunit;

namespace PhotoFrame.Logic.Tests.UI.Views
{
    public class ViewManagerTest
    {
        private IAppModel _AppModelMock;
        private IFrameController _FrameControllerMock;
        private IFrameConfig _FrameConfigMock;

        public ViewManagerTest()
        {
            _AppModelMock = Mock.Of<IAppModel>();
            _FrameControllerMock = Mock.Of<IFrameController>();
            _FrameConfigMock = Mock.Of<IFrameConfig>();
        }

        private ViewManager CreateUUT()
        {
            var result = new ViewManager(_AppModelMock, _FrameControllerMock, _FrameConfigMock);

            return result;
        }

        [Theory]
        [InlineData(ViewType.Border, typeof(ViewBorder))]
        [InlineData(ViewType.Colorized, typeof(ViewColorize))]
        [InlineData(ViewType.Normal, typeof(ViewNormal))]
        public void CreateViewWorks(ViewType viewType, Type expectedViewType)
        {
            //Expect everything that might get relevant for the Views 
            //(as they directly instantiate their ViewModel)
            var frameControllerMock = Mock.Get(_FrameControllerMock);
            var frameConfigMock = Mock.Get(_FrameConfigMock);

            frameControllerMock.Setup(fc => fc.GetNextBorderColor()).Returns(Color.Red);
            frameControllerMock.Setup(fc => fc.GetNextColorizeInfo()).Returns(new ColorizeInfo(0,0,0));
            frameControllerMock.SetupGet(fc => fc.CurrentPhoto).Returns("A photo");
            frameConfigMock.SetupGet(cfg => cfg.BorderWidth).Returns(10u);

            var uut = CreateUUT();
            var view = uut.CreateView(viewType);
            Assert.IsType(expectedViewType, view);
        }

        [Fact]
        public void CreateViewForUnsupportedViewTypeThrows()
        {
            var uut = CreateUUT();
            Assert.Throws<NotSupportedException>(() => uut.CreateView((ViewType)4711));
        }
    }
}
