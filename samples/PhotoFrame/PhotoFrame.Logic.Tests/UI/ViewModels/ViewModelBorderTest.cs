using Moq;
using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;
using PhotoFrame.Logic.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using Xunit;

namespace PhotoFrame.Logic.Tests.UI.ViewModels
{
    public class ViewModelBorderTest : ViewModelTestBase
    {
        protected ViewModelBorder CreateUUT()
        {
            ViewModelBorder result = new ViewModelBorder(_FrameControllerMock, _FrameConfigMock);

            return result;
        }

        private void ExpectViewModelParameters(string imageUri, Color borderColor, uint borderWidth)
        {
            ExpectImageUri(imageUri);
            var frameControllerMock = Mock.Get(_FrameControllerMock);
            frameControllerMock.Setup(fc => fc.GetNextBorderColor()).Returns(borderColor);

            var frameConfigMock = Mock.Get(_FrameConfigMock);
            frameConfigMock.SetupGet(cfg => cfg.BorderWidth).Returns(borderWidth);
        }

        [Fact]
        public void ImageUriIsSetCorrectly()
        {
            ExpectViewModelParameters("ImageUri", Color.FromArgb(0x22, 0x33, 0x44), 42);
            var uut = CreateUUT();

            Assert.Equal("ImageUri", uut.ImageUri);
        }

        [Fact]
        public void BorderColorIsSetCorrectly()
        {
            ExpectViewModelParameters("ImageUri", Color.FromArgb(0x22, 0x33, 0x44), 42);
            var uut = CreateUUT();

            Assert.Equal("#223344", uut.BorderColor);
        }

        [Fact]
        public void BorderWidthIsSetCorrectly()
        {
            ExpectViewModelParameters("ImageUri", Color.FromArgb(0x22, 0x33, 0x44), 42);
            var uut = CreateUUT();

            Assert.Equal(42u, uut.BorderWidth);
        }
    }
}
