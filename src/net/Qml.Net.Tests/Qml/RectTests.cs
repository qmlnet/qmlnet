using System;
using System.Drawing;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Qml.Net.Tests.Qml
{
    public class RectTests : BaseQmlTests<RectTests.RectTestsQml>
    {
        public class RectTestsQml
        {
            public virtual Rectangle Value { get; set; }

            public virtual Rectangle? NullableValue { get; set; }
        }

        [Fact]
        public void Can_get_and_set()
        {
            Mock.SetupGet(x => x.Value).Returns(new Rectangle(1, 2, 3, 4));
            
            RunQmlTest(
                "test",
                @"
                    var v = test.value;
                    // Ensure with a === comparision that it _actually_ is the same type,
                    // and not just a look-alike
                    const expected = Qt.rect(1, 2, 3, 4);
                    if (v !== expected) {
                        throw new Error('Expected to be comparable to rect, but got: ' + v + ' instead of ' + expected);
                    }
                    test.value = Qt.rect(v.x, v.y, v.width, v.height);
                ");

            Mock.VerifySet(x => x.Value = new Rectangle(1, 2, 3, 4));
        }
        
        [Fact]
        public void Can_get_and_set_with_null()
        {
            Mock.SetupGet(x => x.NullableValue).Returns((Rectangle?)null);
            
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