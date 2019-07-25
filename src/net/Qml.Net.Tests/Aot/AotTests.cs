using FluentAssertions;
using Qml.Net.Aot;
using Xunit;

namespace Qml.Net.Tests.Aot
{
    public class AotTests
    {
        public class TestClass
        {
            public void TestMethod1()
            {
                
            }

            public int TestMethod2()
            {
                return 0;
            }
        }
        
        [Fact]
        public void Can_map_class()
        {
            using (var session = new AotSession())
            {
                session.MapClass<TestClass>();
                session.Classes.Count.Should().Be(1);
                session.Classes[0].Type.Should().Be(typeof(TestClass));
            }
        }

        [Fact]
        public void Can_map_method()
        {
            using (var session = new AotSession())
            {
                session.MapClass<TestClass>()
                    .MapMethod(x => x.TestMethod1());
                session.Classes[0].Methods.Count.Should().Be(1);
                session.Classes[0].Methods[0].MethodInfo.Name.Should().Be(nameof(TestClass.TestMethod1));
            }
        }

        [Fact]
        public void Can_map_method_with_return_value()
        {
            using (var session = new AotSession())
            {
                session.MapClass<TestClass>()
                    .MapMethod(x => x.TestMethod2());
                session.Classes[0].Methods.Count.Should().Be(1);
                session.Classes[0].Methods[0].MethodInfo.Name.Should().Be(nameof(TestClass.TestMethod2));
            }
        }
    }
}