#if NETCOREAPP3_1
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class Matrix4x4Tests : BaseQmlTests<Matrix4x4Tests.Matrix4x4TestsQml>
    {
        public class Matrix4x4TestsQml
        {
            public virtual Matrix4x4 Value { get; set; }

            public virtual Matrix4x4? NullableValue { get; set; }
        }

        [Fact]
        public void Can_get_and_set()
        {
            // Note how we can use numbers here that do not have the same representation for double and float,
            // the reason being that Qt's QMatrix4x4 also has single-precision storage (other than QRectF and friends)
            var expectedMatrix = new Matrix4x4(1.1f, 2.2f, 3.3f, 4.4f,
                5.5f, 6.6f, 7.7f, 8.8f,
                9.9f, 10.10f, 11.11f, 12.12f,
                13.13f, 14.14f, 15.15f, 16.16f
            );
            Mock.SetupGet(x => x.Value).Returns(expectedMatrix);
            RunQmlTest(
                "test",
                @"
                    var v = test.value;
                    if (v !== Qt.matrix4x4(1.1, 2.2, 3.3, 4.4,
                                    5.5, 6.6, 7.7, 8.8,
                                    9.9, 10.10, 11.11, 12.12,
                                    13.13, 14.14, 15.15, 16.16)) {
                        throw new Error('Expected QML-type to be comparable to matrix4x4, but got: ' + v);
                    }
                    test.value = Qt.matrix4x4(v.m11, v.m12, v.m13, v.m14,
                                    v.m21, v.m22, v.m23, v.m24,
                                    v.m31, v.m32, v.m33, v.m34,
                                    v.m41, v.m42, v.m43, v.m44);
                ");

            Mock.VerifySet(x => x.Value = expectedMatrix);
        }
        
        [Fact]
        public void Can_get_and_set_with_null()
        {
            Mock.SetupGet(x => x.NullableValue).Returns((Matrix4x4?)null);
            
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