using System;
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
                case ITestAssemblyFinished _:
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
                case IDiscoveryCompleteMessage _:
                    DiscoveryComplete.Set();
                    break;
                case ITestCaseDiscoveryMessage testCaseDiscoveryMessage:
                    Console.WriteLine(testCaseDiscoveryMessage.TestCase.DisplayName);
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
            Console.WriteLine("Discovering...");
            controller.Find(false, sinkWithTypes, discoverOptions);
            sinkWithTypes.DiscoveryComplete.WaitOne();
            
            // Run the tests
            Console.WriteLine("Running...");
            controller.RunTests(sinkWithTypes.TestCases, sink, executionOptions);
            sink.TestAssemblyFinished.WaitOne();
        }
    }
}