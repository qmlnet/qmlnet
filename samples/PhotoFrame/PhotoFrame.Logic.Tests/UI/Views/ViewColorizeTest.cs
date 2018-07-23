using Moq;
using PhotoFrame.Logic.BL;
using PhotoFrame.Logic.UI.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PhotoFrame.Logic.Tests.UI.Views
{
    public class ViewColorizeTest : ViewTestBase<ViewColorize>
    {
        protected override string ExpectedUri => "ViewColorized.qml";

        protected override ViewColorize CreateUUT()
        {
            var frameControllerMock = Mock.Get(_FrameControllerMock);
            frameControllerMock.Setup(fc => fc.GetNextColorizeInfo()).Returns(new ColorizeInfo(0, 0, 0));

            var result = new ViewColorize(_AppModelMock, _FrameControllerMock, _FrameConfigMock);
            return result;
        }
    }
}
