using FluentAssertions;
using Qt.NetCore.Types;
using Xunit;

namespace Qt.NetCore.Tests.Types
{
    public class NetInstanceTests : BaseTests
    {
        class TestObject
        {
            
        }
        
        [Fact]
        public void Can_create_net_instance()
        {
            var o = new TestObject();
            var instance = NetInstance.GetForObject(o);

            var returnedInstance = instance.Instance;

            o.Should().Be(returnedInstance);
        }
    }
}