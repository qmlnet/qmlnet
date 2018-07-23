using PhotoFrame.Logic.UI.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PhotoFrame.Logic.Tests.UI.Views
{
    public class ViewBorderTest : ViewTestBase<ViewBorder>
    {
        protected override string ExpectedUri => "ViewBorder.qml";

        protected override ViewBorder CreateUUT()
        {
            var result = new ViewBorder(_AppModelMock, _FrameControllerMock, _FrameConfigMock);
            return result;
        }
    }
}
