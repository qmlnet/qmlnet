using Moq;
using Qml.Net.Internal.Qml;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class BoolTests : BaseQmlTests<BoolTests.BoolTestsQml>
    {
        public class BoolTestsQml
        {
            public virtual bool Property { get; set; }

            public virtual bool? Nullable { get; set; }
            
            public virtual void MethodParameter(bool value)
            {

            }

            public virtual void MethodParameterNullable(bool? value)
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
                            test.property = true
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
                            test.property = false
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
                            test.methodParameter(test.property)
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
                            test.methodParameter(test.property)
                        }
                    }
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.Verify(x => x.MethodParameter(It.Is<bool>(y => y)));
        }
        
        [Fact]
        public void Can_read_nullable_bool_no_value()
        {
            Mock.Setup(x => x.Nullable).Returns((bool?)null);
            Mock.Setup(x => x.MethodParameterNullable(It.Is<bool?>(y => y == null)));

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    BoolTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.methodParameterNullable(test.nullable)
                        }
                    }
                ");

            Mock.VerifyGet(x => x.Nullable, Times.Once);
            Mock.Verify(x => x.MethodParameterNullable(It.Is<bool?>(y => y == null)), Times.Once);
        }
        
        [Fact]
        public void Can_read_nullable_bool_with_value()
        {
            Mock.Setup(x => x.Nullable).Returns(true);
            Mock.Setup(x => x.MethodParameterNullable(It.Is<bool?>(y => y == true)));

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    BoolTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.methodParameterNullable(test.nullable)
                        }
                    }
                ");

            Mock.VerifyGet(x => x.Nullable, Times.Once);
            Mock.Verify(x => x.MethodParameterNullable(It.Is<bool?>(y => y == true)), Times.Once);
        }
    }
}