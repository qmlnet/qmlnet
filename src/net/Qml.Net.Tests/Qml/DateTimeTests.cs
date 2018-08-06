using Qml.Net.Internal.Qml;
using System;
using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class DateTimeTests : BaseQmlTests<DateTimeTests.DateTimeTestsQml>
    {
        public class DateTimeTestsQml
        {
            public virtual DateTime Property { get; set; }

            public virtual DateTime? Nullable { get; set; }
        }

        [Fact]
        public void Can_read_write_property()
        {
            var value = DateTime.Now;
            Mock.SetupGet(x => x.Property).Returns(value);
            Mock.SetupSet(x => x.Property = value);

            RunQmlTest(
                "test",
                @"
                    test.property = test.property
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = value, Times.Once);
        }
        
        [Fact]
        public void Can_read_write_property_nullable_with_value()
        {
            var value = DateTime.Now;
            Mock.SetupGet(x => x.Nullable).Returns(value);
            Mock.SetupSet(x => x.Nullable = value);

            RunQmlTest(
                "test",
                @"
                    var v = test.nullable
                    test.nullable = v
                ");

            Mock.VerifyGet(x => x.Nullable, Times.Once);
            Mock.VerifySet(x => x.Nullable = value);
        }
        
        [Fact]
        public void Can_read_write_property_nullable_without_value()
        {
            Mock.SetupGet(x => x.Nullable).Returns((DateTime?)null);
            Mock.SetupSet(x => x.Nullable = null);

            RunQmlTest(
                "test",
                @"
                    var v = test.nullable
                    test.nullable = v
                ");

            Mock.VerifyGet(x => x.Nullable, Times.Once);
            Mock.VerifySet(x => x.Nullable = null);
        }
    }
}