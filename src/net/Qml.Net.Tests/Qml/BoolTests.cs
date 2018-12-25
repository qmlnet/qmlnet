using Moq;
using Qml.Net.Internal.Qml;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class BoolTests : BaseQmlTests<BoolTests.BoolTestsQml>
    {
        public class BoolTestsQml
        {
            public virtual bool Property { get; set; }

            public virtual bool? Nullable { get; set; }

            public virtual void MethodParameter(bool value)
            {
            }

            public virtual void MethodParameterNullable(bool? value)
            {
            }

            public virtual bool MethodReturn()
            {
                return false;
            }
        }

        [Fact]
        public void Can_write_property_true()
        {
            RunQmlTest(
                "test",
                @"
                    test.property = true
                ");

            Mock.VerifySet(x => x.Property = true, Times.Once);
        }

        [Fact]
        public void Can_write_property_false()
        {
            RunQmlTest(
                "test",
                @"
                    test.property = false
                ");

            Mock.VerifySet(x => x.Property = false, Times.Once);
        }

        [Fact]
        public void Can_read_property_false()
        {
            Mock.Setup(x => x.Property).Returns(false);

            RunQmlTest(
                "test",
                @"
                    test.methodParameter(test.property)
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.Verify(x => x.MethodParameter(It.Is<bool>(y => y == false)));
        }

        [Fact]
        public void Can_read_property_true()
        {
            Mock.Setup(x => x.Property).Returns(true);

            RunQmlTest(
                "test",
                @"
                    test.methodParameter(test.property)
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.Verify(x => x.MethodParameter(It.Is<bool>(y => y)));
        }

        [Fact]
        public void Can_read_nullable_bool_no_value()
        {
            Mock.Setup(x => x.Nullable).Returns((bool?)null);
            Mock.Setup(x => x.MethodParameterNullable(It.Is<bool?>(y => y == null)));

            RunQmlTest(
                "test",
                @"
                    test.methodParameterNullable(test.nullable)
                ");

            Mock.VerifyGet(x => x.Nullable, Times.Once);
            Mock.Verify(x => x.MethodParameterNullable(It.Is<bool?>(y => y == null)), Times.Once);
        }

        [Fact]
        public void Can_read_nullable_bool_with_value()
        {
            Mock.Setup(x => x.Nullable).Returns(true);
            Mock.Setup(x => x.MethodParameterNullable(It.Is<bool?>(y => y == true)));

            RunQmlTest(
                "test",
                @"
                    test.methodParameterNullable(test.nullable)
                ");

            Mock.VerifyGet(x => x.Nullable, Times.Once);
            Mock.Verify(x => x.MethodParameterNullable(It.Is<bool?>(y => y == true)), Times.Once);
        }
    }
}