using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class MethodTests : BaseQmlTests<MethodTests.MethodTestsQml>
    {
        public class MethodTestsQml : MethodTestsQmlBase
        {
            public virtual void DerivedMethod()
            {
                
            }
        }

        public class MethodTestsQmlBase
        {
            public virtual void BaseMethod()
            {
                
            }
        }

        [Fact]
        public void Can_call_base_method()
        {
            RunQmlTest(
                "test",
                @"
                    test.baseMethod()
                ");

            Mock.Verify(x => x.BaseMethod(), Times.Once);
            Mock.Verify(x => x.DerivedMethod(), Times.Never);
        }
        
        [Fact]
        public void Can_call_derived_method()
        {
            RunQmlTest(
                "test",
                @"
                    test.derivedMethod()
                ");

            Mock.Verify(x => x.BaseMethod(), Times.Never);
            Mock.Verify(x => x.DerivedMethod(), Times.Once);
        }
    }
}