using Moq;
using Qt.NetCore.Qml;
using Xunit;

namespace Qt.NetCore.Tests.Qml
{
    public class IntTests : BaseQmlTests<IntTests.IntTestsQml>
    {
        public class IntTestsQml
        {
            public virtual int Property { get; set; }

            public virtual void MethodParameter(int value)
            {

            }

            public virtual int MethodReturn()
            {
                return 0;
            }
        }
        
        [Fact]
        public void Can_read_write_int_min_value()
        {
            Mock.Setup(x => x.Property).Returns(int.MinValue);
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    IntTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.Property = test.Property
                        }
                    }
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = int.MinValue, Times.Once);
        }

        [Fact]
        public void Can_read_write_int_max_value()
        {
            Mock.Setup(x => x.Property).Returns(int.MaxValue);

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    IntTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.Property = test.Property
                        }
                    }
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = int.MaxValue, Times.Once);
        }

        [Fact]
        public void Can_call_method_with_parameter()
        {
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    IntTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.MethodParameter(3)
                        }
                    }
                ");

            Mock.Verify(x => x.MethodParameter(It.Is<int>(y => y == 3)), Times.Once);
        }

        [Fact]
        public void Can_call_method_with_return()
        {
            Mock.Setup(x => x.MethodReturn()).Returns(int.MaxValue);

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    IntTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.MethodParameter(test.MethodReturn())
                        }
                    }
                ");

            Mock.Verify(x => x.MethodParameter(It.Is<int>(y => y == int.MaxValue)), Times.Once);
        }
    }
}