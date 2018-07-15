using System;
using System.Threading;
using FluentAssertions;
using Xunit;

namespace Qt.NetCore.Tests.Types
{
    public class NetTypeManagerTests : BaseTests
    {
        class TestType
        {
            
        }
        
        [Fact]
        public void Can_get_net_type()
        {
            var typeInfo = NetTypeManager.GetTypeInfo<TestType>();

            typeInfo.FullTypeName.Should().Be(typeof(TestType).AssemblyQualifiedName);
            typeInfo.ClassName.Should().Be("TestType");
        }

        [Fact]
        public void Null_type_returned_for_invalid_type()
        {
            var typeInfo = NetTypeManager.GetTypeInfo("tt");
            typeInfo.Should().BeNull();
        }
    }
}