using Qml.Net.Tests.Qml;

namespace Qml.Net.Tests.Aot
{
    public abstract class AotTestsBase : BaseQmlEngineTests
    {
        protected AotTestsBase()
        {
            // Register the AOT types.
            TestInterop.TestInterop.Register();
        }
    }
}