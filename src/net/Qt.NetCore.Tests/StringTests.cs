using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;

namespace Qt.NetCore.Tests
{
    public class StringTests : BaseTests<StringTests.StringTestsQml>
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

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0

                    StringTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.Property = test.Property
                        }
                    }
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = null, Times.Once);
        }

        [Fact]
        public void Can_read_write_empty()
        {
            Mock.Setup(x => x.Property).Returns("");

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0

                    StringTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.Property = test.Property
                        }
                    }
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = "", Times.Once);
        }

        [Fact]
        public void Can_read_write_value()
        {
            Mock.Setup(x => x.Property).Returns("test value");

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0

                    StringTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.Property = test.Property
                        }
                    }
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = "test value", Times.Once);
        }

        [Fact]
        public void Can_read_write_value_unicode()
        {
            Mock.Setup(x => x.Property).Returns("test Ώ value");

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0

                    StringTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.Property = test.Property
                        }
                    }
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = "test Ώ value", Times.Once);
        }
    }
}
