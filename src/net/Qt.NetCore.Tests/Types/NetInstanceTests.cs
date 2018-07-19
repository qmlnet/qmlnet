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

        [Fact]
        public void Can_create_instance_from_type_info()
        {
            var typeInfo = NetTypeManager.GetTypeInfo<TestObject>();
            var instance = NetInstance.InstantiateType(typeInfo);
            instance.Should().NotBeNull();
            instance.Instance.Should().BeOfType<TestObject>();
        }
    }
}