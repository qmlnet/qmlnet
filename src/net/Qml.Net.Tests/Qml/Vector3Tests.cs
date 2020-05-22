#if NETCOREAPP3_1
using System.Drawing;
using System.Numerics;
using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class Vector3Tests : BaseQmlTests<Vector3Tests.Vector3TestsQml>
    {
        public class Vector3TestsQml
        {
            public virtual Vector3 Value { get; set; }

            public virtual Vector3? NullableValue { get; set; }
        }

        [Fact]
        public void Can_get_and_set()
        {
            // Note how we can use numbers here that do not have the same representation for double and float,
            // the reason being that Qt's Vector3D also has single-precision storage (other than QRectF and friends)
            Mock.SetupGet(x => x.Value).Returns(new Vector3(1.1f, 2.2f, 3.3f));
            
            RunQmlTest(
                "test",
                @"
                    var v = test.value;
                    if (v !== Qt.vector3d(1.1, 2.2, 3.3)) {
                        throw new Error('Expected QML-type to be comparable to vector3d, but got: ' + v);
                    }
                    test.value = Qt.vector3d(v.x, v.y, v.z);
                ");

            Mock.VerifySet(x => x.Value = new Vector3(1.1f, 2.2f, 3.3f));
        }
        
        [Fact]
        public void Can_get_and_set_with_null()
        {
            Mock.SetupGet(x => x.NullableValue).Returns((Vector3?)null);
            
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