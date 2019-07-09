using System.Runtime.CompilerServices;
using Moq;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class ByteArrayTests : BaseQmlTests<ByteArrayTests.ByteArrayTestsQml>
    {
        public class ByteArrayTestsQml
        {
            public virtual byte[] Array { get; set; }
        }

        [Fact]
        public void Can_get_and_set_byte_array()
        {
            Mock.SetupGet(x => x.Array).Returns(new byte[] { 3, 5 });
            
            RunQmlTest(
                "test",
                @"
                    var v = test.array
                    var data = new Uint8Array(v)
                    data.set([4], 0)
                    test.array = v
                ");

            Mock.VerifyGet(x => x.Array, Times.Once);
            Mock.VerifySet(x => x.Array = new byte[]{4,5});
        }
        
        [Fact]
        public void Can_get_and_set_byte_array_with_null()
        {
            Mock.SetupGet(x => x.Array).Returns((byte[])null);
            
            RunQmlTest(
                "test",
                @"
                    test.array = test.array
                ");

            Mock.VerifyGet(x => x.Array, Times.Once);
            Mock.VerifySet(x => x.Array = null);
        }
        
        [Fact]
        public void Can_set_byte_array_null()
        {
            RunQmlTest(
                "test",
                @"
                    test.array = null
                ");

            Mock.VerifySet(x => x.Array = null);
        }
    }
}