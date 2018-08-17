using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class ArrayTests : BaseQmlTests<ArrayTests.ArrayTestsQml>
    {
        public class ArrayTestsQml
        {
            public virtual int[] GetArray()
            {
                return null;
            }

            public virtual void Test(object value)
            {
                
            }
        }

        [Fact]
        public void Can_get_length()
        {
            var array = new[] {3, 4, 6};
            Mock.Setup(x => x.GetArray()).Returns(array);
            Mock.Setup(x => x.Test(3));
            
            RunQmlTest(
                "test",
                @"
                    var array = Net.toJsArray(test.getArray())
                    test.test(array.length)
                ");
            
            Mock.Verify(x => x.GetArray(), Times.Once);
            Mock.Verify(x => x.Test(3), Times.Once);
        }
    }
}