using FluentAssertions;
using Xunit;

namespace Qml.Net.Tests.Aot
{
    public class NetReferenceTests : AotTestsBase
    {
        [Fact]
        public void Can_detect_aot_type()
        {
            var aotType = new AotMethodInvocationTests.AotMethodInvocation();
            var nonAotType = this;
            
            using (var netReference = global::Qml.Net.Internal.Types.NetReference.CreateForObject(aotType))
            {
                netReference.IsAot.Should().BeTrue();
            }
            
            using(var netReference = global::Qml.Net.Internal.Types.NetReference.CreateForObject(nonAotType))
            {
                netReference.IsAot.Should().BeFalse();
            }
        }
    }
}