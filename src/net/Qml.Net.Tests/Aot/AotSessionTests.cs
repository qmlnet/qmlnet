using FluentAssertions;
using Qml.Net.Aot;
using Xunit;

namespace Qml.Net.Tests.Aot
{
    public class AotSessionTests
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
        
        public class TestClass2
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

        [Fact]
        public void Can_generate_type_id()
        {
            using (var session = new AotSession())
            {
                session.MapClass<TestClass>();
                session.MapClass<TestClass2>();

                session.Classes[0].TypeId.Should().Be(1);
                session.Classes[1].TypeId.Should().Be(2);
            }
        }
    }
}