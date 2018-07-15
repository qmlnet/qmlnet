using Moq;
using Qt.NetCore.Qml;
using Xunit;

namespace Qt.NetCore.Tests
{
    public class UIntTests : BaseQmlTests<UIntTests.UIntTestsQml>
    {
        public class UIntTestsQml
        {
            public virtual uint Property { get; set; }

            public virtual void MethodParameter(uint value)
            {

            }

            public virtual uint MethodReturn()
            {
                return 0;
            }
        }

        [Fact]
        public void Can_read_write_int_min_value()
        {
            Mock.Setup(x => x.Property).Returns(uint.MinValue);

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                import QtQuick 2.0
                import tests 1.0
                UIntTestsQml {
                    id: test
                    Component.onCompleted: function() {
                        test.Property = test.Property
                    }
                }
            ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = uint.MinValue, Times.Once);
        }

        [Fact]
        public void Can_read_write_int_max_value()
        {
            Mock.Setup(x => x.Property).Returns(uint.MaxValue);

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                import QtQuick 2.0
                import tests 1.0
                UIntTestsQml {
                    id: test
                    Component.onCompleted: function() {
                        test.Property = test.Property
                    }
                }
            ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = uint.MaxValue, Times.Once);
        }

        [Fact]
        public void Can_call_method_with_parameter()
        {
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                import QtQuick 2.0
                import tests 1.0
                UIntTestsQml {
                    id: test
                    Component.onCompleted: function() {
                        test.MethodParameter(3)
                    }
                }
            ");

            Mock.Verify(x => x.MethodParameter(It.Is<uint>(y => y == 3)), Times.Once);
        }

        [Fact]
        public void Can_call_method_with_return()
        {
            Mock.Setup(x => x.MethodReturn()).Returns(uint.MaxValue);

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                import QtQuick 2.0
                import tests 1.0
                UIntTestsQml {
                    id: test
                    Component.onCompleted: function() {
                        test.MethodParameter(test.MethodReturn())
                    }
                }
            ");

            Mock.Verify(x => x.MethodParameter(It.Is<uint>(y => y == uint.MaxValue)), Times.Once);
        }
    }
}