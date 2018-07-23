using Moq;
using PhotoFrame.Logic.UI.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace PhotoFrame.Logic.Tests.UI.Views
{
    public class ViewNormalTest : ViewTestBase<ViewNormal>
    {
        protected override string ExpectedUri => "ViewNormal.qml";

        protected override ViewNormal CreateUUT()
        {
            var result = new ViewNormal(_AppModelMock, _FrameControllerMock, _FrameConfigMock);
            return result;
        }
    }
}
