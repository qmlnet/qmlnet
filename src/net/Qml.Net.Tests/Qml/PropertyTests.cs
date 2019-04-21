using Moq;
using Qml.Net.Internal.Qml;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class PropertyTests : BaseQmlTests<PropertyTests.PropertyTestsQml>
    {
        public class PropertyTestsQml : PropertyTestsQmlBase
        {
            public virtual bool DerivedProperty { get; set; }
        }

        public class PropertyTestsQmlBase
        {
            public virtual bool BaseProperty { get; set; }
        }

        [Fact]
        public void Can_set_derived_property()
        {
            RunQmlTest(
                "test",
                @"
                    test.derivedProperty = true
                ");

            Mock.VerifySet(x => x.DerivedProperty = true, Times.Once);
            Mock.VerifySet(x => x.BaseProperty = true, Times.Never);
        }
        
        [Fact]
        public void Can_get_derived_property()
        {
            RunQmlTest(
                "test",
                @"
                    var t = test.derivedProperty
                ");

            Mock.VerifyGet(x => x.DerivedProperty, Times.Once);
            Mock.VerifyGet(x => x.BaseProperty, Times.Never);
        }
        
        [Fact]
        public void Can_set_base_property()
        {
            RunQmlTest(
                "test",
                @"
                    test.baseProperty = true
                ");

            Mock.VerifySet(x => x.DerivedProperty = true, Times.Never);
            Mock.VerifySet(x => x.BaseProperty = true, Times.Once);
        }
        
        [Fact]
        public void Can_get_base_property()
        {
            RunQmlTest(
                "test",
                @"
                    var t = test.baseProperty
                ");

            Mock.VerifyGet(x => x.DerivedProperty, Times.Never);
            Mock.VerifyGet(x => x.BaseProperty, Times.Once);
        }
    }
}