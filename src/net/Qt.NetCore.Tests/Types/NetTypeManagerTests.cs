using FluentAssertions;
using Xunit;

namespace Qt.NetCore.Tests.Types
{
    public class NetTypeManagerTests
    {
        class TestType
        {
            
        }
        
        [Fact]
        public void Can_get_net_type()
        {
            var typeInfo = NetTypeManager.GetTypeInfo<TestType>();

            typeInfo.FullTypeName.Should().Be(typeof(TestType).AssemblyQualifiedName);
        }
    }
}