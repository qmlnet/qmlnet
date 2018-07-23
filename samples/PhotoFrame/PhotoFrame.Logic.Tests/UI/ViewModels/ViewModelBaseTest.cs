using Moq;
using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.Config;
using PhotoFrame.Logic.UI.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PhotoFrame.Logic.Tests.UI.ViewModels
{
    public class ViewModelBaseTest : ViewModelTestBase
    {
        public class ViewModelBaseTestClass : ViewModelBase
        {
            public IFrameController ExposedFrameController
            {
                get
                {
                    return FrameController;
                }
            }

            public IFrameConfig ExposedFrameConfig
            {
                get
                {
                    return FrameConfig;
                }
            }

            public ViewModelBaseTestClass(IFrameController frameController, IFrameConfig frameConfig) 
                : base(frameController, frameConfig)
            {
            }
        };

        protected ViewModelBase CreateUUT()
        {
            ViewModelBase result = new ViewModelBaseTestClass(_FrameControllerMock, _FrameConfigMock);

            return result;
        }

        [Fact]
        public void ImageUriGetsSetCorrectly()
        {
            ExpectImageUri("This is a URI");
            var uut = CreateUUT();
            Assert.Equal("This is a URI", uut.ImageUri);
        }

        [Fact]
        public void ImageUriChangesGetPropagatedCorrectly()
        {
            var frameControllerMock = Mock.Get(_FrameControllerMock);

            ExpectImageUri("ImageUri");
            var uut = CreateUUT();

            string catchedImageUri = "";
            uut.PropertyChanged += (s, e) => 
            {
                if(e.PropertyName == "ImageUri")
                {
                    catchedImageUri = ((ViewModelBase)s).ImageUri;
                }
            };

            ExpectImageUri("ImageUri 2");
            frameControllerMock.Raise(fc => fc.CurrentPhotoChanged += null, new CurrentPhotoChangedEventArgs());

            Assert.Equal("ImageUri 2", catchedImageUri);
        }

        [Fact]
        public void ImageUriChangesDontGetPropagatedAfterStopCorrectly()
        {
            var frameControllerMock = Mock.Get(_FrameControllerMock);

            ExpectImageUri("ImageUri");
            var uut = CreateUUT();

            string catchedImageUri = null;
            uut.PropertyChanged += (s, e) => 
            {
                if(e.PropertyName == "ImageUri")
                {
                    catchedImageUri = ((ViewModelBase)s).ImageUri;
                }
            };

            ExpectImageUri("ImageUri 2");
            uut.Stop();
            frameControllerMock.Raise(fc => fc.CurrentPhotoChanged += null, new CurrentPhotoChangedEventArgs());

            Assert.Null(catchedImageUri);
        }
    }
}
