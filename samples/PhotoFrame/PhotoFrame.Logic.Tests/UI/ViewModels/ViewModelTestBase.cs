using Moq;
using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace PhotoFrame.Logic.Tests.UI.ViewModels
{
    public class ViewModelTestBase
    {
        protected IFrameController _FrameControllerMock;
        protected IFrameConfig _FrameConfigMock;

        public ViewModelTestBase()
        {
            _FrameControllerMock = Mock.Of<IFrameController>();
            _FrameConfigMock = Mock.Of<IFrameConfig>();
        }

        protected void ExpectImageUri(string imageUri)
        {
            var frameControllerMock = Mock.Get(_FrameControllerMock);
            frameControllerMock.SetupGet(fc => fc.CurrentPhoto).Returns(imageUri);
        }

    }
}
