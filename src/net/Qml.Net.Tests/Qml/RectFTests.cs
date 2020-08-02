using System.Drawing;
using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class RectFTests : BaseQmlTests<RectFTests.RectFTestsQml>
    {
        public class RectFTestsQml
        {
            public virtual RectangleF Value { get; set; }

            public virtual RectangleF? NullableValue { get; set; }
        }

        [Fact]
        public void Can_get_and_set()
        {
            Mock.SetupGet(x => x.Value).Returns(new RectangleF(0.5f, 0.25f, 0.125f, 0.0625f));
            
            RunQmlTest(
                "test",
                @"
                    var v = test.value;
                    // Ensure with a === comparision that it _actually_ is the same type,
                    // and not just a look-alike
                    const expected = Qt.rect(0.5, 0.25, 0.125, 0.0625);
                    if (v !== expected) {
                        throw new Error('Expected to be comparable to rect, but got: ' + v + ' instead of ' + expected);
                    }
                    test.value = Qt.rect(v.x, v.y, v.width, v.height);
                ");

            Mock.VerifySet(x => x.Value = new RectangleF(0.5f, 0.25f, 0.125f, 0.0625f));
        }
        
        [Fact]
        public void Can_get_and_set_with_null()
        {
            Mock.SetupGet(x => x.NullableValue).Returns((RectangleF?)null);
            
            RunQmlTest(
                "test",
                @"
                    test.nullableValue = test.nullableValue
                ");

            Mock.VerifyGet(x => x.NullableValue, Times.Once);
            Mock.VerifySet(x => x.NullableValue = null);
        }
        
        [Fact]
        public void Can_set_null()
        {
            RunQmlTest(
                "test",
                @"
                    test.nullableValue = null
                ");

            Mock.VerifySet(x => x.NullableValue = null);
        }
    }
}