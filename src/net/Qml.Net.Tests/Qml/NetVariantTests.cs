using System;
using FluentAssertions;
using Qml.Net.Internal.Qml;
using Qml.Net.Internal.Types;
using Xunit;

namespace Qml.Net.Tests.Qml
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
            variant.Instance = NetReference.CreateForObject(testObject);
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
        
        [Fact]
        public void Can_store_double()
        {
            var variant = new NetVariant();
            variant.Double = double.MinValue;
            variant.VariantType.Should().Be(NetVariantType.Double);
            variant.Double.Should().Be(double.MinValue);
            variant.Double = double.MaxValue;
            variant.Double.Should().Be(double.MaxValue);
        }
        
        [Fact]
        public void Can_store_string()
        {
            var variant = new NetVariant();
            variant.String.Should().BeNull();
            variant.String = "test";
            variant.VariantType.Should().Be(NetVariantType.String);
            variant.String.Should().Be("test");
            variant.String = "";
            variant.String.Should().Be("");
            variant.String = null;
            variant.String.Should().BeNull();
        }
        
        [Fact]
        public void Can_store_date()
        {
            var variant = new NetVariant();
            variant.DateTime.Should().BeNull();
            variant.DateTime = new DateTimeOffset(1988, 9, 3, 0, 0, 0, 0, TimeSpan.FromHours(5));
            variant.VariantType.Should().Be(NetVariantType.DateTime);
            var value = variant.DateTime;
            value.Should().NotBeNull();
            value.Value.Year.Should().Be(1988);
            value.Value.Month.Should().Be(9);
            value.Value.Day.Should().Be(3);
            value.Value.Hour.Should().Be(0);
            value.Value.Minute.Should().Be(0);
            value.Value.Second.Should().Be(0);
            value.Value.Millisecond.Should().Be(0);
            value.Value.Offset.Should().Be(TimeSpan.FromHours(5));
        }

        [Fact]
        public void Can_clear_value()
        {
            var variant = new NetVariant();
            variant.String = "test";
            variant.VariantType.Should().Be(NetVariantType.String);
            variant.Clear();
            variant.VariantType.Should().Be(NetVariantType.Invalid);
        }
    }
}