using System;
using System.Drawing;
using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class SizeTests : BaseQmlTests<SizeTests.SizeTestsQml>
    {
        public class SizeTestsQml
        {
            public virtual Size Value { get; set; }

            public virtual Size? NullableValue { get; set; }
        }

        [Fact]
        public void Can_get_and_set()
        {
            Mock.SetupGet(x => x.Value).Returns(new Size(1, 2));
            
            RunQmlTest(
                "test",
                @"
                    var v = test.value;
                    if (v !== Qt.size(1, 2)) {
                        throw new Error('Expected QML-type to be comparable to size, but got: ' + v);
                    }
                    test.value = Qt.size(v.width, v.height);
                ");

            Mock.VerifySet(x => x.Value = new Size(1, 2));
        }
        
        [Fact]
        public void Can_get_and_set_with_null()
        {
            Mock.SetupGet(x => x.NullableValue).Returns((Size?)null);
            
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