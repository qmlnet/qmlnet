#if NETCOREAPP3_1
using System.Drawing;
using System.Numerics;
using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class Vector2Tests : BaseQmlTests<Vector2Tests.Vector2TestsQml>
    {
        public class Vector2TestsQml
        {
            public virtual Vector2 Value { get; set; }

            public virtual Vector2? NullableValue { get; set; }
        }

        [Fact]
        public void Can_get_and_set()
        {
            // Note how we can use numbers here that do not have the same representation for double and float,
            // the reason being that Qt's Vector2D also has single-precision storage (other than QRectF and friends)
            Mock.SetupGet(x => x.Value).Returns(new Vector2(1.1f, 2.2f));
            
            RunQmlTest(
                "test",
                @"
                    var v = test.value;
                    if (v !== Qt.vector2d(1.1, 2.2)) {
                        throw new Error('Expected QML-type to be comparable to vector2d, but got: ' + v);
                    }
                    test.value = Qt.vector2d(v.x, v.y);
                ");

            Mock.VerifySet(x => x.Value = new Vector2(1.1f, 2.2f));
        }
        
        [Fact]
        public void Can_get_and_set_with_null()
        {
            Mock.SetupGet(x => x.NullableValue).Returns((Vector2?)null);
            
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
#endif