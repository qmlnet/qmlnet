using System.Runtime.CompilerServices;
using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class EnumTests : BaseQmlTests<EnumTests.EnumTestsObject>
    {
        public class EnumTestsObject
        {
            public enum TestEnum
            {
                Value1,
                Value2,
                Value3
            }

            public virtual TestEnum Value { get; set; }

            public virtual TestEnum? ValueNullable { get; set; }

            public virtual void Test(string value)
            {
            }
        }

        [Fact]
        public void Can_use_enum()
        {
            Mock.Setup(x => x.Value).Returns(EnumTestsObject.TestEnum.Value2);

            RunQmlTest(
                "test",
                @"
                    test.value = test.value
                ");

            Mock.VerifyGet(x => x.Value, Times.Once);
            Mock.VerifySet(x => x.Value = EnumTestsObject.TestEnum.Value2, Times.Once);
        }

        [Fact]
        public void Can_set_enum_to_int()
        {
            Mock.Setup(x => x.Value);

            RunQmlTest(
                "test",
                @"
                    test.value = 2
                ");

            Mock.VerifySet(x => x.Value = EnumTestsObject.TestEnum.Value3, Times.Once);
        }

        [Fact]
        public void Can_use_nullable_enum()
        {
            Mock.Setup(x => x.ValueNullable).Returns(EnumTestsObject.TestEnum.Value2);

            RunQmlTest(
                "test",
                @"
                    test.valueNullable = test.valueNullable
                ");

            Mock.VerifyGet(x => x.ValueNullable, Times.Once);
            Mock.VerifySet(x => x.ValueNullable = EnumTestsObject.TestEnum.Value2, Times.Once);
        }

        [Fact]
        public void Can_use_nullable_enum_with_null()
        {
            Mock.Setup(x => x.ValueNullable).Returns((EnumTestsObject.TestEnum?)null);

            RunQmlTest(
                "test",
                @"
                    test.valueNullable = test.valueNullable
                ");

            Mock.VerifyGet(x => x.ValueNullable, Times.Once);
            Mock.VerifySet(x => x.ValueNullable = null, Times.Once);
        }

        [Fact]
        public void Enum_is_int_type_is_js()
        {
            Mock.Setup(x => x.Value).Returns(EnumTestsObject.TestEnum.Value1);
            Mock.Setup(x => x.Test(It.IsAny<string>()));

            RunQmlTest(
                "test",
                @"
                    test.test(typeof test.value)
                ");

            Mock.VerifyGet(x => x.Value, Times.Once);
            Mock.Verify(x => x.Test(It.Is<string>(value => value == "number")), Times.Once);
        }

        [Fact]
        public void Nullable_enum_is_int_type_is_js()
        {
            Mock.Setup(x => x.ValueNullable).Returns(EnumTestsObject.TestEnum.Value1);
            Mock.Setup(x => x.Test(It.IsAny<string>()));

            RunQmlTest(
                "test",
                @"
                    test.test(typeof test.valueNullable)
                ");

            Mock.VerifyGet(x => x.ValueNullable, Times.Once);
            Mock.Verify(x => x.Test(It.Is<string>(value => value == "number")), Times.Once);
        }

        [Fact]
        public void Nullable_enum_is_null_type_is_js_when_null()
        {
            Mock.Setup(x => x.ValueNullable).Returns((EnumTestsObject.TestEnum?)null);
            Mock.Setup(x => x.Test(It.IsAny<string>()));

            RunQmlTest(
                "test",
                @"
                    test.test(test.valueNullable == null)
                ");

            Mock.VerifyGet(x => x.ValueNullable, Times.Once);
            Mock.Verify(x => x.Test(It.Is<string>(value => value == "true")), Times.Once);
        }
    }
}