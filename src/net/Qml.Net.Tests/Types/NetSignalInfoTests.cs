using FluentAssertions;
using Qml.Net.Internal.Types;
using Xunit;

namespace Qml.Net.Tests.Types
{
    public class NetSignalInfoTests
    {
        [Fact]
        public void Can_create_signal_info()
        {
            using (var type = new NetTypeInfo("full type name"))
            using (var signal = new NetSignalInfo(type, "testSignal"))
            {
                signal.ParentType.FullTypeName.Should().Be("full type name");
                signal.Name.Should().Be("testSignal");
                signal.ParameterCount.Should().Be(0);
                signal.GetParameter(0).Should().Be(NetVariantType.Invalid);
                signal.AddParameter(NetVariantType.Double);
                signal.ParameterCount.Should().Be(1);
                signal.GetParameter(0).Should().Be(NetVariantType.Double);
                signal.GetParameter(1).Should().Be(NetVariantType.Invalid);
            }
        }
    }
}