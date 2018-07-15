using FluentAssertions;
using Xunit;

namespace Qt.NetCore.Tests.Types
{
    public class NetTypeInfoTests : BaseTests
    {
        [Fact]
        public void Can_create_type()
        {
            var typeInfo = new NetTypeInfo("fullTypeName");
            typeInfo.FullTypeName.Should().Be("fullTypeName");
        }
    }
}