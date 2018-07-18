using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Qt.NetCore.Qml;
using Qt.NetCore.Types;
using Xunit;

namespace Qt.NetCore.Tests.Types
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
            NetInstance instance = null;
            Task.Factory.StartNew(() =>
            {
                var o = new TestObject();
                reference = new WeakReference(o);
                instance = NetInstance.CreateFromObject(o);
            }).Wait();

            // NetInstance is still alive, so the weak reference must be alive as well.
            GC.Collect(GC.MaxGeneration);
            reference.IsAlive.Should().BeTrue();
            
            instance.Dispose();
            
            // NetInstance has been destroyed, so the handle should have been released.
            GC.Collect(GC.MaxGeneration);
            reference.IsAlive.Should().BeFalse();
        }
        
        [Fact]
        public void Can_instantiate_type()
        {
            var type = NetTypeManager.GetTypeInfo<TestObject>();

            using(var instance = new NetInstance(Interop.Callbacks.InstantiateType(type.FullTypeName), type))
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
            var method = type.GetMethod(0); 
            var instance = NetInstance.CreateFromObject(o); 
            
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