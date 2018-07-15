using FluentAssertions;
using Qt.NetCore.Qml;
using Xunit;

namespace Qt.NetCore.Tests
{
    public class NetVariantTests : BaseTests
    {
        [Fact]
        public void Variant_is_invalid_by_default()
        {
            var variant = new NetVariant();
            variant.VariantType.Should().Be(NetVariantType.Invalid);
        }
        
        [Fact]
        public void Can_store_string()
        {
            
        }
    }
}