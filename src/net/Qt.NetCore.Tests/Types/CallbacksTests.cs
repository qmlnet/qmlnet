using AdvancedDLSupport;
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
            callbacks.Setup(x => x.IsTypeValid());
            
            Interop.RegisterCallbacks(callbacks.Object);
            Interop.Callbacks.isTypeValid();
            
            callbacks.Verify(x => x.IsTypeValid(), Times.Once);
        }
    }
}