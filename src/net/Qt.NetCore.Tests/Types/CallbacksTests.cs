using AdvancedDLSupport;
using FluentAssertions;
using Moq;
using Xunit;

namespace Qt.NetCore.Tests.Types
{
    public class CallbacksTests
    {
        [Fact]
        public void Can_register_callbacks()
        {
            var callbacks = new Mock<ICallbacks>();
            callbacks.Setup(x => x.IsTypeValid("test-type")).Returns(true);
            
            Interop.RegisterCallbacks(callbacks.Object);
            Interop.Callbacks.IsTypeValid("test-type").Should().BeTrue();
            
            callbacks.Verify(x => x.IsTypeValid("test-type"), Times.Once);
        }
    }
}