using System;
using AdvancedDLSupport;
using FluentAssertions;
using Moq;
using Xunit;

namespace Qt.NetCore.Tests.Types
{
    public class CallbacksTests
    {
        [Fact]
        public void Can_call_is_type_valid()
        {
            var callbacks = new Mock<ICallbacks>();
            callbacks.Setup(x => x.IsTypeValid("test-type")).Returns(true);
            
            Interop.RegisterCallbacks(callbacks.Object);
            Interop.Callbacks.IsTypeValid("test-type").Should().BeTrue();
            
            callbacks.Verify(x => x.IsTypeValid("test-type"), Times.Once);
        }

        [Fact]
        public void Can_build_type_info()
        {
            var callbacks = new Mock<ICallbacks>();
            NetTypeInfo typeInfo = null;
            callbacks.Setup(x => x.BuildTypeInfo(It.IsAny<NetTypeInfo>())).Callback(new Action<NetTypeInfo>(x => typeInfo = x));
            
            Interop.RegisterCallbacks(callbacks.Object);
            Interop.Callbacks.BuildTypeInfo(new NetTypeInfo("test").Handle);

            callbacks.Verify(x => x.BuildTypeInfo(It.IsAny<NetTypeInfo>()), Times.Once);
            typeInfo.FullTypeName.Should().Be("test");
        }
    }
}