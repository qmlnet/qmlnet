using System;
using FluentAssertions;
using Xunit;

namespace Qt.NetCore.Tests.Types
{
    public class NetTypeInfoTests
    {
        [Fact]
        public void Can_create_type()
        {
            var typeInfo = new NetTypeInfo("fullTypeName");

            typeInfo.FullTypeName.Should().Be("fullTypeName");
        }
    }
}