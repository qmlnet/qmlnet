using FluentAssertions;
using Qt.NetCore.Types;
using Xunit;

namespace Qt.NetCore.Tests
{
    public class NetInstanceTests
    {
        class TestObject
        {
            
        }
        
        [Fact]
        public void Can_create_net_instance()
        {
            //var o = new TestObject();
            //var instance = NetInstance.CreateFromObject(o);

            //var returnedInstance = instance.Instance;

            //o.Should().Be(returnedInstance);
        }

        [Fact]
        public void Can_create_instance_from_type_info()
        {
            //var typeInfo = NetTypeManager.GetTypeInfo<TestObject>();
            //var instance = NetTypeManager.InstantiateType(typeInfo);
            //instance.Should().NotBeNull();
            //instance.Instance.Should().BeOfType<TestObject>();
        }
    }
}