using System;
using System.Drawing;
using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class SizeFTests : BaseQmlTests<SizeFTests.SizeFTestsQml>
    {
        public class SizeFTestsQml
        {
            public virtual SizeF Value { get; set; }

            public virtual SizeF? NullableValue { get; set; }
        }

        [Fact]
        public void Can_get_and_set()
        {
            Mock.SetupGet(x => x.Value).Returns(new SizeF(0.5f, 0.25f));
            
            RunQmlTest(
                "test",
                @"
                    var v = test.value;
                    // Ensure with a === comparision that it _actually_ is the same type,
                    // and not just a look-alike
                    const expected = Qt.size(0.5, 0.25);
                    if (v !== expected) {
                        throw new Error('Expected to be comparable to size, but got: ' + v + ' instead of ' + expected);
                    }
                    test.value = Qt.size(v.width, v.height);
                ");

            Mock.VerifySet(x => x.Value = new SizeF(0.5f, 0.25f));
        }
        
        [Fact]
        public void Can_get_and_set_with_null()
        {
            Mock.SetupGet(x => x.NullableValue).Returns((SizeF?)null);
            
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