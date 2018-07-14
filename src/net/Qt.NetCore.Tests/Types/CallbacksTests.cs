using AdvancedDLSupport;
using Xunit;

namespace Qt.NetCore.Tests.Types
{
    public class CallbacksTests
    {
        [Fact]
        public void Can_register_callbacks()
        {
            var callbacks = new CallbacksImpl();
            Interop.Callbacks.registerCallbacks();
        }
    }
}