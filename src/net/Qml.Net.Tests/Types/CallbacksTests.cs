using System;
using System.Threading.Tasks;
using FluentAssertions;
using Qml.Net.Internal;
using Qml.Net.Internal.Qml;
using Qml.Net.Internal.Types;
using Xunit;

namespace Qml.Net.Tests.Types
{
    public class CallbacksTests : BaseTests
    {
        class TestObject
        {
            public void Method(TestObject o)
            {
                Object = o;
            }

            public TestObject Object { get; set; }
        }

        [Fact]
        public void Can_call_is_type_valid()
        {
            Interop.Callbacks.IsTypeValid("none-type").Should().BeFalse();
            Interop.Callbacks.IsTypeValid(typeof(TestObject).AssemblyQualifiedName).Should().BeTrue();
        }

        [Fact]
        public void Can_release_gc_handle()
        {
            WeakReference reference = null;
            NetReference instance = null;
            Task.Factory.StartNew(() =>
            {
                var o = new TestObject();
                reference = new WeakReference(o);
                instance = NetReference.CreateForObject(o);
            }).Wait();

            // NetReference is still alive, so the weak reference must be alive as well.
            GC.Collect(GC.MaxGeneration);
            reference.IsAlive.Should().BeTrue();

            instance.Dispose();

            // NetReference has been destroyed, so the handle should have been released.
            GC.Collect(GC.MaxGeneration);
            reference.IsAlive.Should().BeFalse();
        }

        [Fact]
        public void Can_instantiate_type()
        {
            var type = NetTypeManager.GetTypeInfo<TestObject>();

            using (var instance = new NetReference(Interop.Callbacks.InstantiateType(type.Handle), false))
            {
                instance.Instance.Should().NotBeNull();
                instance.Instance.Should().BeOfType<TestObject>();
            }
        }

        [Fact]
        public void Can_invoke_method()
        {
            var o = new TestObject();
            var type = NetTypeManager.GetTypeInfo<TestObject>();
            type.EnsureLoaded();
            var method = type.GetMethod(0);
            var instance = NetReference.CreateForObject(o);

            // This will jump to native, to then call the .NET delegate (round trip).
            // The purpose is to simulate Qml invoking a method, sending .NET instance back.
            // We will inspect the returned instance that it got back to verify that it
            using (var parameter = new NetVariant())
            using (var list = new NetVariantList())
            {
                parameter.Instance = instance;
                list.Add(parameter);
                Interop.Callbacks.InvokeMethod(method.Handle, instance.Handle, list.Handle, IntPtr.Zero);
            }

            o.Object.Should().NotBeNull();
            ReferenceEquals(o.Object, o).Should().BeTrue();
        }
    }
}