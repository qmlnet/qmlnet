using Qml.Net.Aot;
using Qml.Net.Tests.Qml;
using Xunit;
using Xunit.Sdk;

namespace Qml.Net.Tests.Aot
{
    public class AotMethodInvocationTests : BaseQmlTests<AotMethodInvocationTests.AotMethodInvocation>
    {
        public class Mapper : IAotSessionMapper
        {
            public void MapSession(AotSession session)
            {
                session.MapClass<AotMethodInvocation>()
                    .MapMethod(x => x.Method1())
                    .MapMethod(x => x.Method2());
                // Note: Method3 isn't mapped, for testing.
            }
        }
        
        public class AotMethodInvocation
        {
            public void Method1()
            {
                
            }

            public int Method2()
            {
                return 0;
            }

            public void Method3()
            {
                
            }

            public void Assert()
            {
                
            }
        }

        [Fact]
        public void Can_detect_only_registered_methods()
        {
            RunQmlTest("test",
                @"
                    console.log(test);
                    return;
                    assert.isTrue(typeof test.method1 === 'function');
                    assert.isTrue(typeof test.method2 !== 'function');
                    assert.isTrue(typeof test.method3 === 'function');
                ");
        }
    }
}