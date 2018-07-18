using Moq;
using Qt.NetCore.Qml;
using Xunit;

namespace Qt.NetCore.Tests.Qml
{
    public class BoolTests : BaseQmlTests<BoolTests.BoolTestsQml>
    {
        public class BoolTestsQml
        {
            public virtual bool Property { get; set; }

            public virtual void MethodParameter(bool value)
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
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    BoolTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.Property = true
                        }
                    }
                ");
            
            Mock.VerifySet(x => x.Property = true, Times.Once);
        }

        [Fact]
        public void Can_write_property_false()
        {
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    BoolTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.Property = false
                        }
                    }
                ");

            Mock.VerifySet(x => x.Property = false, Times.Once);
        }

        [Fact]
        public void Can_read_property_false()
        {
            Mock.Setup(x => x.Property).Returns(false);

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    BoolTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.MethodParameter(test.Property)
                        }
                    }
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.Verify(x => x.MethodParameter(It.Is<bool>(y => y == false)));
        }

        [Fact]
        public void Can_read_property_true()
        {
            Mock.Setup(x => x.Property).Returns(true);

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    BoolTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.MethodParameter(test.Property)
                        }
                    }
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.Verify(x => x.MethodParameter(It.Is<bool>(y => y)));
        }
    }
}