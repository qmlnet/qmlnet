using System.Collections.Generic;
using System.Threading;
using Xunit;
using Xunit.Abstractions;

namespace Qt.NetCore.Sandbox
{
    class Sink : IMessageSink
    {
        public bool OnMessage(IMessageSinkMessage message)
        {
            switch (message)
            {
                case Xunit.Sdk.TestAssemblyFinished _:
                    TestAssemblyFinished.Set();
                    break;
            }
            return true;
        }
        
        public readonly ManualResetEvent TestAssemblyFinished = new ManualResetEvent(false);
    }

    class SinkWithTypes : IMessageSinkWithTypes
    {
        public void Dispose()
        {
            
        }

        public bool OnMessageWithTypes(IMessageSinkMessage message, HashSet<string> messageTypes)
        {
            switch (message)
            {
                case DiscoveryCompleteMessage _:
                    DiscoveryComplete.Set();
                    break;
                case Xunit.Sdk.TestCaseDiscoveryMessage testCaseDiscoveryMessage:
                    TestCases.Add(testCaseDiscoveryMessage.TestCase);
                    break;
            }

            return true;
        }

        public readonly ManualResetEvent DiscoveryComplete = new ManualResetEvent(false);

        public readonly List<ITestCase> TestCases = new List<ITestCase>();
    }
    
    class Program
    {
        static void Main()
        {
            var config = ConfigReader.Load(typeof(Tests.BaseTests).Assembly.Location);
            var controller = new XunitFrontController(AppDomainSupport.Denied, typeof(Tests.BaseTests).Assembly.Location);
            var discoverOptions = TestFrameworkOptions.ForDiscovery(config);
            var executionOptions = TestFrameworkOptions.ForExecution(config);
            
            var sinkWithTypes = new SinkWithTypes();
            var sink = new Sink();
            
            // Discover the tests
            controller.Find(false, sinkWithTypes, discoverOptions);
            sinkWithTypes.DiscoveryComplete.WaitOne();
            
            // Run the tests
            controller.RunTests(sinkWithTypes.TestCases, sink, executionOptions);
            sink.TestAssemblyFinished.WaitOne();
        }
    }
}