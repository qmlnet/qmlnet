using Qml.Net.Aot;
using Xunit;

namespace Qml.Net.Tests.Aot
{
    public class AotMethodInvocationTests : AotTestsBase
    {
        public class TestClass
        {
            public void Method1()
            {
                
            }

            public int Method2()
            {
                return 0;
            }
        }

        public override void MapSession(AotSession session)
        {
            session.MapClass<TestClass>()
                .MapMethod(x => x.Method1())
                .MapMethod(x => x.Method2());
        }

        [Fact]
        public void Can_invoke_method()
        {
            
        }
    }
}