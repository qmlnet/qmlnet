using System;
using Moq;
using Xunit;

namespace Qt.NetCore.Tests
{
    public class DateTimeTests : BaseTests<DateTimeTests.DateTimeTestsQml>
    {
        public class DateTimeTestsQml
        {
            public virtual DateTime Property { get; set; }

            public virtual void MethodParameter(DateTime value)
            {

            }

            public virtual DateTime MethodReturn()
            {
                return DateTime.Now;
            }
        }

        [Fact]
        public void Can_read_date_time_utc()
        {
            var time = DateTime.UtcNow;
            var timeReturned = (DateTime?) null;
            Mock.Setup(x => x.Property).Returns(time);
            Mock.Setup(x => x.MethodParameter(It.IsAny<DateTime>()))
                .Callback(new Action<DateTime>(result =>
                {
                    timeReturned = result;
                }));

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    DateTimeTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.MethodParameter(test.Property)
                        }
                    }
                ");

            // NOTE: When qml handles dates, it always uses local time, so that is what we get back.
            // I'd like to get what I passed in, but there is no way to tell if original
            // DateTime value was UTC or not.
            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.Verify(x => x.MethodParameter(It.IsAny<DateTime>()));
            Assert.NotNull(timeReturned);
            Assert.Equal(time.ToUniversalTime().ToString("MM/dd/yyyy hh:mm:ss"), timeReturned.Value.ToUniversalTime().ToString("MM/dd/yyyy hh:mm:ss"));
        }

        [Fact]
        public void Can_read_date_time_local()
        {
            var time = DateTime.Now;
            var timeReturned = (DateTime?)null;
            Mock.Setup(x => x.Property).Returns(time);
            Mock.Setup(x => x.MethodParameter(It.IsAny<DateTime>()))
                .Callback(new Action<DateTime>(result =>
                {
                    timeReturned = result;
                }));

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    DateTimeTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.MethodParameter(test.Property)
                        }
                    }
                ");
            
            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.Verify(x => x.MethodParameter(It.IsAny<DateTime>()));
            Assert.NotNull(timeReturned);
            Assert.Equal(time.ToString("MM/dd/yyyy hh:mm:ss"), timeReturned.Value.ToString("MM/dd/yyyy hh:mm:ss"));
        }
    }
}