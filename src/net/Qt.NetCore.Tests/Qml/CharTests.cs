using Moq;
using Qt.NetCore.Qml;
using Xunit;

namespace Qt.NetCore.Tests.Qml
{
    public class CharTests : BaseQmlTests<CharTests.CharTestsQml>
    {
        public class CharTestsQml
        {
            public virtual char Property { get; set; }

            public virtual void MethodParameter(char value)
            {

            }

            public virtual char MethodReturn()
            {
                return char.MinValue;
            }
        }
        
        [Fact]
        public void Can_read_write_char_null()
        {
            Mock.Setup(x => x.Property).Returns((char)0);
            
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    CharTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.Property = test.Property
                        }
                    }
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = (char)0, Times.Once);
        }

        [Fact]
        public void Can_read_write_char_max_value()
        {
            Mock.Setup(x => x.Property).Returns('T');

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    CharTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.Property = test.Property
                        }
                    }
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = 'T', Times.Once);
        }

        [Fact]
        public void Can_read_write_char_unicode()
        {
            Mock.Setup(x => x.Property).Returns('Ώ');

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    CharTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.Property = test.Property
                        }
                    }
                ");

            Mock.VerifyGet(x => x.Property, Times.Once);
            Mock.VerifySet(x => x.Property = 'Ώ', Times.Once);
        }

        [Fact]
        public void Can_set_method_parameter()
        {
            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    CharTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.MethodParameter(""Ώ"")
                        }
                    }
                ");

            Mock.Verify(x => x.MethodParameter(It.IsIn('Ώ')), Times.Once);
        }

        [Fact]
        public void Can_use_as_return_type()
        {
            Mock.Setup(x => x.MethodReturn()).Returns('Ώ');

            NetTestHelper.RunQml(qmlApplicationEngine,
                @"
                    import QtQuick 2.0
                    import tests 1.0
                    CharTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            test.MethodParameter(test.MethodReturn())
                        }
                    }
                ");

            Mock.Verify(x => x.MethodParameter(It.IsIn('Ώ')), Times.Once);
        }
    }
}