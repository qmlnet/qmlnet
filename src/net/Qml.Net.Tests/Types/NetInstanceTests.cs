using FluentAssertions;
using Qml.Net.Types;
using Xunit;

namespace Qml.Net.Tests.Types
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