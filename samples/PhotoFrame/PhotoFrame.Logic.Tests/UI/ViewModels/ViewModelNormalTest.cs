using PhotoFrame.Logic.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PhotoFrame.Logic.Tests.UI.ViewModels
{
    public class ViewModelNormalTest : ViewModelTestBase
    {
        protected ViewModelNormal CreateUUT()
        {
            var result = new ViewModelNormal(_FrameControllerMock, _FrameConfigMock);
            return result;
        }

        [Fact]
        public void ImageUriIsSetCorrectly()
        {
            ExpectImageUri("Another ImageUri");
            var uut = CreateUUT();
            Assert.Equal("Another ImageUri", uut.ImageUri);
        }
    }
}
