using System;
using System.Runtime.InteropServices;
using FluentAssertions;
using Moq;
using Xunit;

namespace Qt.NetCore.Tests.Types
{
    public class CallbacksTests : BaseTests
    {
        [Fact]
        public void Can_call_is_type_valid()
        {
            try
            {
                var callbacks = new Mock<ICallbacks>();
                callbacks.Setup(x => x.IsTypeValid("test-type")).Returns(true);

                Interop.RegisterCallbacks(callbacks.Object);
                Interop.Callbacks.IsTypeValid("test-type").Should().BeTrue();

                callbacks.Verify(x => x.IsTypeValid("test-type"), Times.Once);
            }
            finally
            {
                Interop.SetDefaultCallbacks();
            }
        }

        [Fact]
        public void Can_build_type_info()
        {
            try
            {
                var callbacks = new Mock<ICallbacks>();
                var type = IntPtr.Zero;
                callbacks.Setup(x => x.BuildTypeInfo(It.IsAny<IntPtr>()))
                    .Callback(new Action<IntPtr>(x => type = x));

                Interop.RegisterCallbacks(callbacks.Object);
                Interop.Callbacks.BuildTypeInfo(new IntPtr(3));

                callbacks.Verify(x => x.BuildTypeInfo(It.IsAny<IntPtr>()), Times.Once);
                type.Should().Be(new IntPtr(3));
            }
            finally
            {
                Interop.SetDefaultCallbacks();
            }
        }

        [Fact]
        public void Can_release_gc_handle()
        {
            try
            {
                var callbacks = new Mock<ICallbacks>();
                IntPtr handle = IntPtr.Zero;
                callbacks.Setup(x => x.ReleaseGCHandle(It.IsAny<IntPtr>()))
                    .Callback(new Action<IntPtr>(x => handle = x));

                Interop.RegisterCallbacks(callbacks.Object);
                Interop.Callbacks.ReleaseGCHandle(new IntPtr(3));

                callbacks.Verify(x => x.ReleaseGCHandle(It.IsAny<IntPtr>()), Times.Once);
                handle.Should().Be(new IntPtr(3));
            }
            finally
            {
                Interop.SetDefaultCallbacks();
            }
        }
        
        [Fact]
        public void Can_instantiate_type()
        {
            try
            {
                var callbacks = new Mock<ICallbacks>();
                string typeName = null;
                callbacks.Setup(x => x.InstantiateType(It.IsAny<string>()))
                    .Callback(new Action<string>(x => typeName = x))
                    .Returns((GCHandle)new IntPtr(3));

                Interop.RegisterCallbacks(callbacks.Object);
                var result = Interop.Callbacks.InstantiateType("test");

                callbacks.Verify(x => x.InstantiateType(It.IsAny<string>()), Times.Once);
                typeName.Should().Be("test");
                result.Should().Be(new IntPtr(3));
            }
            finally
            {
                Interop.SetDefaultCallbacks();
            }
        }
    }
}