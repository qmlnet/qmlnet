using FluentAssertions;
using Qt.NetCore.Types;
using Xunit;

namespace Qt.NetCore.Tests.Types
{
    public class NetTypeInfoTests : BaseTests
    {
        [Fact]
        public void Can_create_type()
        {
            var typeInfo = new NetTypeInfo("fullTypeName");
            typeInfo.FullTypeName.Should().Be("fullTypeName");
        }

        [Fact]
        public void Can_add_signals()
        {
            var type = new NetTypeInfo("test");
            var signal = new NetSignalInfo(type, "signalName");
            signal.AddParameter(NetVariantType.Bool);

            type.GetSignal(0).Should().BeNull();
            type.SignalCount.Should().Be(0);
            type.AddSignal(signal);
            type.SignalCount.Should().Be(1);
            type.GetSignal(0).Name.Should().Be("signalName");
            type.GetSignal(0).GetParameter(0).Should().Be(NetVariantType.Bool);
        }
    }
}