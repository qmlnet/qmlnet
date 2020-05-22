#if NETCOREAPP3_1
using System.Drawing;
using System.Numerics;
using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class Vector4Tests : BaseQmlTests<Vector4Tests.Vector4TestsQml>
    {
        public class Vector4TestsQml
        {
            public virtual Vector4 Value { get; set; }

            public virtual Vector4? NullableValue { get; set; }
        }

        [Fact]
        public void Can_get_and_set()
        {
            // Note how we can use numbers here that do not have the same representation for double and float,
            // the reason being that Qt's Vector4D also has single-precision storage (other than QRectF and friends)
            Mock.SetupGet(x => x.Value).Returns(new Vector4(1.1f, 2.2f, 3.3f, 4.4f));
            
            RunQmlTest(
                "test",
                @"
                    var v = test.value;
                    if (v !== Qt.vector4d(1.1, 2.2, 3.3, 4.4)) {
                        throw new Error('Expected QML-type to be comparable to vector4d, but got: ' + v);
                    }
                    test.value = Qt.vector4d(v.x, v.y, v.z, v.w);
                ");

            Mock.VerifySet(x => x.Value = new Vector4(1.1f, 2.2f, 3.3f, 4.4f));
        }
        
        [Fact]
        public void Can_get_and_set_with_null()
        {
            Mock.SetupGet(x => x.NullableValue).Returns((Vector4?)null);
            
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