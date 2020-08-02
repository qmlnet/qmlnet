using System;
using System.Threading.Tasks;
using FluentAssertions;
using Qml.Net.Internal.Qml;
using Qml.Net.Internal.Types;
using Qml.Net.Tests.Types;
using Xunit;

namespace Qml.Net.Tests.CodeGen
{
    public class CodeGenStructTests
    {
        public struct TestObject
        {
            public string TestProperty { get; set; }
        }

        [Fact]
        public void Can_get_property()
        {
            try
            {
                var value = new TestObject();
                value.TestProperty = "value1";

                var typeInfo = NetTypeManager.GetTypeInfo<TestObject>();
                typeInfo.EnsureLoaded();

                var readProperty =
                    global::Qml.Net.Internal.CodeGen.CodeGen.BuildReadPropertyDelegate(typeInfo.GetProperty(0));

                NetVariant result = new NetVariant();
                Task taskResult = null;
                readProperty(NetReference.CreateForObject(value), new NetVariantList(), result, ref taskResult);

                result.String.Should().Be("value1");
                
                throw new Exception("This shouldn't be ran");
            }
            catch (Exception ex)
            {
                ex.Message.Should().StartWith("Can't operate on struct types yet");
            }
        }
    }
}