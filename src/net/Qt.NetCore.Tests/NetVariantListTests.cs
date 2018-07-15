using FluentAssertions;
using Qt.NetCore.Qml;
using Xunit;

namespace Qt.NetCore.Tests
{
    public class NetVariantListTests : BaseTests
    {
        [Fact]
        public void Can_create_net_variant_list()
        {
            using (var list = new NetVariantList())
            {
                list.Count.Should().Be(0);
            }
        }

        [Fact]
        public void Can_add_variants()
        {
            using (var list = new NetVariantList())
            {
                using (var variant = new NetVariant())
                {
                    variant.String = "test";
                    list.Add(variant);
                    list.Count.Should().Be(1);
                    list.Get(0).String.Should().Be("test");
                }
            }
        }

        [Fact]
        public void Can_remove_variants()
        {
            using (var list = new NetVariantList())
            {
                using (var variant1 = new NetVariant())
                using (var variant2 = new NetVariant())
                {
                    variant1.String = "test1";
                    variant2.String = "test2";
                    
                    list.Add(variant1);
                    list.Add(variant2);
                    
                    list.Count.Should().Be(2);
                    
                    list.Remove(0);

                    list.Count.Should().Be(1);
                    list.Get(0).String.Should().Be("test2");
                }
            }
        }

        [Fact]
        public void Can_clear_variants()
        {
            using (var list = new NetVariantList())
            {
                using (var variant1 = new NetVariant())
                using (var variant2 = new NetVariant())
                {
                    variant1.String = "test1";
                    variant2.String = "test2";
                    
                    list.Add(variant1);
                    list.Add(variant2);
                    
                    list.Count.Should().Be(2);
                    
                    list.Clear();

                    list.Count.Should().Be(0);
                }
            }
        }
    }
}