using Moq;
using Qml.Net.Internal.Qml;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class ObjectTests : BaseQmlTests<ObjectTests.ObjectTestsQml>
    {
        public class ObjectTestsQml
        {
            public virtual ObjectTestsQmlReturnType TestMethodReturn()
            {
                return new ObjectTestsQmlReturnType();
            }

            public virtual void TestMethodParameter(ObjectTestsQmlReturnType parameter)
            {
                
            }

            public virtual void Overload()
            {
                
            }


            public virtual void Overload(string param)
            {
                
            }
            
            public object TestObjectProperty { get; set; }

            public virtual void TestObjectPropertyTest(string result)
            {
                
            }
        }

        public class ObjectTestsQmlReturnType
        {
            public virtual void TestMethod()
            {
                
            }
        }

        [Fact]
        public void Can_return_object()
        {
            var returnedType = new Mock<ObjectTestsQmlReturnType>();
            Mock.Setup(x => x.TestMethodReturn()).Returns(returnedType.Object);

            RunQmlTest(
                "test",
                @"
                    var instance = test.testMethodReturn()
                    instance.testMethod()
                ");

            returnedType.Verify(x => x.TestMethod(), Times.Once);
            Mock.Verify(x => x.TestMethodReturn(), Times.Once);
        }

        [Fact]
        public void Can_pass_object_to_method()
        {
            var returnedType = new ObjectTestsQmlReturnType();
            Mock.Setup(x => x.TestMethodReturn()).Returns(returnedType);

            RunQmlTest(
                "test",
                @"
                    var instance = test.testMethodReturn()
                    test.testMethodParameter(instance)
                ");

            Mock.Verify(x => x.TestMethodReturn(), Times.Once);
            Mock.Verify(x => x.TestMethodParameter(It.Is<ObjectTestsQmlReturnType>(y => y == returnedType)), Times.Once);
        }

        [Fact]
        public void Can_call_correct_overload()
        {
            Mock.Setup(x => x.Overload());
            Mock.Setup(x => x.Overload(It.IsAny<string>()));

            RunQmlTest(
                "test",
                @"
                    test.overload()
                ");
            
            Mock.Verify(x => x.Overload(), Times.Once);
            Mock.Verify(x => x.Overload(It.IsAny<string>()), Times.Never);
            
            RunQmlTest(
                "test",
                @"
                    test.overload('test')
                ");
            
            Mock.Verify(x => x.Overload(), Times.Once);
            Mock.Verify(x => x.Overload(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public void Can_get_null()
        {
            Mock.Setup(x => x.TestObjectPropertyTest(It.IsAny<string>()));
            
            RunQmlTest(
                "test",
                @"
                    test.testObjectPropertyTest(typeof test.testObjectProperty)
                ");
            
            Mock.Verify(x => x.TestObjectPropertyTest("object"), Times.Once);
        }
    }
}