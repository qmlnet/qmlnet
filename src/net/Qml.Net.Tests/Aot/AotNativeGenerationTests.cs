using Qml.Net.Aot;
using Xunit;

namespace Qml.Net.Tests.Aot
{
    public class AotNativeGenerationTests : AotTestsBase
    {
        public class Test
        {
            public void TestMethod()
            {
                
            }
        }
        
        [Fact]
        public void Can_generate_native_code()
        {
            GenerateNativeBuildFiles(aotSession =>
            {
                aotSession.MapClass<Test>().MapMethod(x => x.TestMethod());
            });

            RunNativeBuild();
        }
    }
}