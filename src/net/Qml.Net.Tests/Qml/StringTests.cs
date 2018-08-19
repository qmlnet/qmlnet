using Moq;
using Qml.Net.Internal.Qml;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class StringTests : BaseQmlTests<StringTests.StringTestsQml>
    {
        public class StringTestsQml
        {
            public virtual string Property { get; set; }

            public virtual void MethodParameter(string value)
            {

            }

            public virtual string MethodReturn()
            {
                return null;
            }
        }

        [Fact]
        public void Can_read_write_null()
        {
            Mock.Setup(x => x.Property).Returns((string)null);

            RunQmlTest(
                "test",
                @"
                    test.property = test.property
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = null, Times.Once);
        }

        [Fact]
        public void Can_read_write_empty()
        {
            Mock.Setup(x => x.Property).Returns("");

            RunQmlTest(
                "test",
                @"
                    test.property = test.property
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = "", Times.Once);
        }

        [Fact]
        public void Can_read_write_value()
        {
            Mock.Setup(x => x.Property).Returns("test value");

            RunQmlTest(
                "test",
                @"
                    test.property = test.property
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = "test value", Times.Once);
        }

        [Fact]
        public void Can_read_write_value_unicode()
        {
            Mock.Setup(x => x.Property).Returns("test Ώ value");

            RunQmlTest(
                "test",
                @"
                    test.property = test.property
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = "test Ώ value", Times.Once);
        }
    }
}