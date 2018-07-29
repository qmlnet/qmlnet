using System;
using Moq;
using Qml.Net.Internal.Qml;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class DateTimeOffsetTests : BaseQmlTests<DateTimeOffsetTests.DateTimeOffsetTestsQml>
    {
        public class DateTimeOffsetTestsQml
        {
            public virtual DateTimeOffset Property { get; set; }

            public virtual DateTimeOffset? Nullable { get; set; }

            public virtual void Method(DateTimeOffset value)
            {
                
            }
            
            public virtual void MethodNullable(DateTimeOffset? value)
            {
                
            }
        }

        [Fact]
        public void Can_read_write_property()
        {
            var value = DateTimeOffset.Now;
            // This trims some percision off of the milliseconds, makes the comparison accurate.
            value = new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Offset);
            Mock.SetupGet(x => x.Property).Returns(value);
            Mock.SetupSet(x => x.Property = value);

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                import QtQuick 2.0
                import tests 1.0
                DateTimeOffsetTestsQml {
                    id: test
                    Component.onCompleted: function() {
                        test.Property = test.Property
                    }
                }
            ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = value, Times.Once);
        }
        
        [Fact]
        public void Can_read_write_property_nullable_with_value()
        {
            var value = DateTimeOffset.Now;
            // This trims some percision off of the milliseconds, makes the comparison accurate.
            value = new DateTimeOffset(value.Year, value.Month, value.Day, value.Hour, value.Minute, value.Second, value.Millisecond, value.Offset);
            Mock.SetupGet(x => x.Nullable).Returns(value);
            Mock.SetupSet(x => x.Nullable = value);

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    DateTimeOffsetTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            var v = test.Nullable
                            test.Nullable = v
                        }
                    }
                ");

            Mock.VerifyGet(x => x.Nullable, Times.Once);
            Mock.VerifySet(x => x.Nullable = value);
        }
        
        [Fact]
        public void Can_read_write_property_nullable_without_value()
        {
            Mock.SetupGet(x => x.Nullable).Returns((DateTimeOffset?)null);
            Mock.SetupSet(x => x.Nullable = null);

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    DateTimeOffsetTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            var v = test.Nullable
                            test.Nullable = v
                        }
                    }
                ");

            Mock.VerifyGet(x => x.Nullable, Times.Once);
            Mock.VerifySet(x => x.Nullable = null);
        }
    }
}