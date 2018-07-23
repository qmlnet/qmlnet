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
    public class TestView : ViewBase
    {
        public IViewModel ViewModelMock { get; private set; }

        public IFrameController ExposedFrameController
        {
            get
            {
                return FrameController;
            }
        }

        public IFrameConfig ExposedConfig
        {
            get
            {
                return FrameConfig;
            }
        }

        public TestView(string uri, IAppModel appModel, IFrameController frameController, IFrameConfig config)
            : base(uri, appModel, frameController, config)
        {
            ViewModelMock = Mock.Of<IViewModel>();
        }

        protected override IViewModel CreateViewModel()
        {
            return ViewModelMock;
        }
    }

    public class ViewBaseTest : ViewTestBase<TestView>
    {
        public ViewBaseTest()
        {
        }

        protected override TestView CreateUUT()
        {
            var result = new TestView(ExpectedUri, _AppModelMock, _FrameControllerMock, _FrameConfigMock);
            return result;
        }

        protected override string ExpectedUri => "Uri";

        [Fact]
        void ConstructorDoesNothing()
        {
            var uut = CreateUUT();
        }

        [Fact]
        void FrameControllerIsProvidedToSubClasses()
        {
            var uut = CreateUUT();

            Assert.Same(_FrameControllerMock, uut.ExposedFrameController);
        }

        [Fact]
        void DeactivationWorks()
        {
            var uut = CreateUUT();

            uut.Activate(ViewSwitchType.Fade);

            var viewModelMock = Mock.Get(uut.ViewModelMock);
            viewModelMock.Setup(vm => vm.Stop());
            uut.Deactivate();

            viewModelMock.Verify(
                vm => vm.Stop(),
                Times.Once);
        }
    }
}
