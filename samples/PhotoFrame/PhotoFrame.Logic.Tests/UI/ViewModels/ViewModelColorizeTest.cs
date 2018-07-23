using Moq;
using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PhotoFrame.Logic.Tests.UI.ViewModels
{
    public class ViewModelColorizeTest : ViewModelTestBase
    {
        protected void ExpectViewModelParameters(string imageUri, double hue, double saturation, double lightness)
        {
            ExpectImageUri(imageUri);

            var frameControllerMock = Mock.Get(_FrameControllerMock);
            frameControllerMock.Setup(fc => fc.GetNextColorizeInfo()).Returns(new ColorizeInfo(hue, saturation, lightness));
        }

        protected ViewModelColorize CreateUUT()
        {
            var result = new ViewModelColorize(_FrameControllerMock, _FrameConfigMock);

            return result;
        }

        [Fact]
        public void ImageUriIsSetCorrectly()
        {
            ExpectViewModelParameters("This is an ImageUri", 1, 0.5d, 2.3d);
            var uut = CreateUUT();
            Assert.Equal("This is an ImageUri", uut.ImageUri);
        }

        [Fact]
        public void HueIsSetCorrectly()
        {
            ExpectViewModelParameters("This is an ImageUri", 1, 0.5d, 2.3d);
            var uut = CreateUUT();
            Assert.Equal(1, uut.Hue);
        }

        [Fact]
        public void SaturationIsSetCorrectly()
        {
            ExpectViewModelParameters("This is an ImageUri", 1, 0.5d, 2.3d);
            var uut = CreateUUT();
            Assert.Equal(0.5d, uut.Saturation);
        }

        [Fact]
        public void LightnessIsSetCorrectly()
        {
            ExpectViewModelParameters("This is an ImageUri", 1, 0.5d, 2.3d);
            var uut = CreateUUT();
            Assert.Equal(2.3d, uut.Lightness);
        }
    }
}
