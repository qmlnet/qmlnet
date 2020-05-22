using System;
using System.Drawing;
using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class PointFTests : BaseQmlTests<PointFTests.PointFTestsQml>
    {
        public class PointFTestsQml
        {
            public virtual PointF Value { get; set; }

            public virtual PointF? NullableValue { get; set; }
        }

        [Fact]
        public void Can_get_and_set()
        {
            Mock.SetupGet(x => x.Value).Returns(new PointF(.5f, .25f));
            
            RunQmlTest(
                "test",
                @"
                    var v = test.value;
                    // Ensure with a === comparision that it _actually_ is the same type,
                    // and not just a look-alike
                    const expected = Qt.point(0.5, 0.25);
                    if (v !== expected) {
                        throw new Error('Expected to be comparable to point, but got: ' + v + ' instead of ' + expected);
                    }
                    test.value = Qt.point(v.x, v.y);
                ");

            Mock.VerifySet(x => x.Value = new PointF(.5f, .25f));
        }
        
        [Fact]
        public void Can_get_and_set_with_null()
        {
            Mock.SetupGet(x => x.NullableValue).Returns((PointF?)null);
            
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