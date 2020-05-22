using System;
using System.Drawing;
using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class PointTests : BaseQmlTests<PointTests.PointTestsQml>
    {
        public class PointTestsQml
        {
            public virtual Point Value { get; set; }

            public virtual Point? NullableValue { get; set; }
        }

        [Fact]
        public void Can_get_and_set()
        {
            Mock.SetupGet(x => x.Value).Returns(new Point(1, 2));
            
            RunQmlTest(
                "test",
                @"
                    var v = test.value;
                    // Ensure with a === comparision that it _actually_ is the same type,
                    // and not just a look-alike
                    const expected = Qt.point(1, 2);
                    if (v !== expected) {
                        throw new Error('Expected to be comparable to point, but got: ' + v + ' instead of ' + expected);
                    }
                    test.value = Qt.point(v.x, v.y);
                ");

            Mock.VerifySet(x => x.Value = new Point(1, 2));
        }
        
        [Fact]
        public void Can_get_and_set_with_null()
        {
            Mock.SetupGet(x => x.NullableValue).Returns((Point?)null);
            
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