using Moq;
using Qt.NetCore.Qml;
using Xunit;

namespace Qt.NetCore.Tests
{
    public class DoubleTests : BaseQmlTests<DoubleTests.DoubleTestsQml>
    {
        public class DoubleTestsQml
        {
            public virtual double Property { get; set; }

            public virtual void MethodParameter(double value)
            {

            }

            public virtual double MethodReturn()
            {
                return 0;
            }
        }
        
        [Fact]
        public void Can_read_write_int_min_value()
        {
            Mock.Setup(x => x.Property).Returns(double.MinValue);
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    DoubleTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.Property = test.Property
                        }
                    }
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = double.MinValue, Times.Once);
        }

        [Fact]
        public void Can_read_write_int_max_value()
        {
            Mock.Setup(x => x.Property).Returns(double.MaxValue);

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    DoubleTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.Property = test.Property
                        }
                    }
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = double.MaxValue, Times.Once);
        }

        [Fact]
        public void Can_call_method_with_parameter()
        {
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    DoubleTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.MethodParameter(3)
                        }
                    }
                ");

            Mock.Verify(x => x.MethodParameter(It.Is<double>(y => y == 3D)), Times.Once);
        }

        [Fact]
        public void Can_call_method_with_return()
        {
            Mock.Setup(x => x.MethodReturn()).Returns(double.MaxValue);

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    DoubleTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.MethodParameter(test.MethodReturn())
                        }
                    }
                ");

            Mock.Verify(x => x.MethodParameter(It.Is<double>(y => y == double.MaxValue)), Times.Once);
        }
    }
}