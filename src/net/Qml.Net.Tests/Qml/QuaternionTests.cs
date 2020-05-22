#if NETCOREAPP3_1
using System.Drawing;
using System.Numerics;
using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class QuaternionTests : BaseQmlTests<QuaternionTests.QuaternionTestsQml>
    {
        public class QuaternionTestsQml
        {
            public virtual Quaternion Value { get; set; }

            public virtual Quaternion? NullableValue { get; set; }
        }

        [Fact]
        public void Can_get_and_set()
        {
            // Note how we can use numbers here that do not have the same representation for double and float,
            // the reason being that Qt's QQuaternion also has single-precision storage (other than QRectF and friends)
            Mock.SetupGet(x => x.Value).Returns(new Quaternion(1.1f, 2.2f, 3.3f, 100.1f));
            
            RunQmlTest(
                "test",
                @"
                    var v = test.value;
                    if (v !== Qt.quaternion(100.1, 1.1, 2.2, 3.3)) {
                        throw new Error('Expected QML-type to be comparable to quaternion, but got: ' + v);
                    }
                    test.value = Qt.quaternion(v.scalar, v.x, v.y, v.z);
                ");

            Mock.VerifySet(x => x.Value = new Quaternion(1.1f, 2.2f, 3.3f, 100.1f));
        }
        
        [Fact]
        public void Can_get_and_set_with_null()
        {
            Mock.SetupGet(x => x.NullableValue).Returns((Quaternion?)null);
            
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