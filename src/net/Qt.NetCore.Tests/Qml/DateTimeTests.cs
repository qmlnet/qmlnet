using System;
using Qt.NetCore.Qml;
using Xunit;

namespace Qt.NetCore.Tests.Qml
{
    public class DateTimeTests : BaseQmlTests<DateTimeTests.DateTimeTestsQml>
    {
        public class DateTimeTestsQml
        {
            public virtual DateTime Property { get; set; }

        }

        [Fact]
        public void Can_read_date_time_utc()
        {
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    DateTimeTestsQml {
                        id: test
                        Component.onCompleted: function() {
                        }
                    }
                ");

        }

        [Fact]
        public void Can_read_date_time_local()
        {
           

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    DateTimeTestsQml {
                        id: test
                        Component.onCompleted: function() {
                        }
                    }
                ");
        }
    }
}