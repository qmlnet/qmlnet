using FluentAssertions;
using Qt.NetCore.Qml;
using Qt.NetCore.Types;
using Xunit;

namespace Qt.NetCore.Tests
{
    public class NetVariantTests : BaseTests
    {
        public class TestObject
        {
            
        }
        
        [Fact]
        public void Variant_is_invalid_by_default()
        {
            var variant = new NetVariant();
            variant.VariantType.Should().Be(NetVariantType.Invalid);
        }
        
        [Fact]
        public void Can_store_net_instance()
        {
            var testObject = new TestObject();
            var variant = new NetVariant();
            variant.Instance.Should().BeNull();
            variant.Instance = NetInstance.CreateFromObject(testObject);
            variant.Instance.Should().NotBeNull();
            variant.Instance.Instance.Should().Be(testObject);
            variant.VariantType.Should().Be(NetVariantType.Object);
        }

        [Fact]
        public void Can_store_bool()
        {
            var variant = new NetVariant();
            variant.Bool = true;
            variant.VariantType.Should().Be(NetVariantType.Bool);
            variant.Bool.Should().BeTrue();
            variant.Bool = false;
            variant.Bool.Should().BeFalse();
        }

        [Fact]
        public void Can_store_char()
        {
            var variant = new NetVariant();
            variant.Char = 'Ώ';
            variant.VariantType.Should().Be(NetVariantType.Char);
            variant.Char.Should().Be('Ώ');
            variant.Char = ' ';
            variant.Char.Should().Be(' ');
        }
        
        [Fact]
        public void Can_store_int()
        {
            var variant = new NetVariant();
            variant.Int = -1;
            variant.VariantType.Should().Be(NetVariantType.Int);
            variant.Int.Should().Be(-1);
            variant.Int = int.MaxValue;
            variant.Int.Should().Be(int.MaxValue);
        }
        
        [Fact]
        public void Can_store_uint()
        {
            var variant = new NetVariant();
            variant.UInt = uint.MinValue;
            variant.VariantType.Should().Be(NetVariantType.UInt);
            variant.UInt.Should().Be(uint.MinValue);
            variant.UInt = uint.MaxValue;
            variant.UInt.Should().Be(uint.MaxValue);
        }
    }
}