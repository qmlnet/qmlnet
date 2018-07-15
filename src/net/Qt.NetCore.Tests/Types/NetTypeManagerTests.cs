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
            public void TestMethodNoReturn()
            {
                
            }

            public int TestMethodReturn()
            {
                return 0;
            }
        }
        
        [Fact]
        public void Can_get_net_type()
        {
            var typeInfo = NetTypeManager.GetTypeInfo<TestType>();

            typeInfo.FullTypeName.Should().Be(typeof(TestType).AssemblyQualifiedName);
            typeInfo.ClassName.Should().Be("TestType");
            typeInfo.PrefVariantType.Should().Be(NetVariantType.Object);

            typeInfo.MethodCount.Should().Be(2);
        }

        [Fact]
        public void Null_type_returned_for_invalid_type()
        {
            var typeInfo = NetTypeManager.GetTypeInfo("tt");
            typeInfo.Should().BeNull();
        }
    }
}