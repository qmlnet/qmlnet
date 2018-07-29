using System.Threading;
using System.Threading.Tasks;
using Moq;
using Qml.Net.Internal.Qml;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class AwaitTests : BaseQmlTests<AwaitTests.AwaitTestsQml>
    {
        public class AwaitTestsQml
        {
            public virtual Task TestAsync()
            {
                return Task.CompletedTask;
            }
            
            public virtual Task<string> TestAsyncWithResult()
            {
                return Task.FromResult("");
            }

            public virtual void TestMethod()
            {
                
            }

            public virtual void TestMethodWithArg(string arg)
            {
                
            }
        }

        [Fact]
        public void Can_await_on_task()
        {
            var oldContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(new NoAsyncSynchronizationContext());

            try
            {
                Mock.Setup(x => x.TestAsync()).Returns(Task.CompletedTask);
                Mock.Setup(x => x.TestMethod());

                NetTestHelper.RunQml(qmlApplicationEngine,
                    @"
                    import QtQuick 2.0
                    import tests 1.0
                    AwaitTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            var task = test.testAsync()
                            Net.await(task, function() {
                                test.testMethod()
                            })
                        }
                    }
                ");

                Mock.Verify(x => x.TestMethod(), Times.Once);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldContext);
            }
        }
        
        [Fact]
        public void Can_await_on_task_with_result()
        {
            var oldContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(new NoAsyncSynchronizationContext());

            try
            {
                Mock.Setup(x => x.TestAsync()).Returns(Task.FromResult("testt"));
                Mock.Setup(x => x.TestMethodWithArg("testt"));

                NetTestHelper.RunQml(qmlApplicationEngine,
                    @"
                    import QtQuick 2.0
                    import tests 1.0
                    AwaitTestsQml {
                        id: test
                        Component.onCompleted: function() {
                            var task = test.testAsync()
                            Net.await(task, function(result) {
                                test.testMethodWithArg(result)
                            })
                        }
                    }
                ");

                Mock.Verify(x => x.TestMethodWithArg("testt"), Times.Once);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldContext);
            }
        }

        private class NoAsyncSynchronizationContext : SynchronizationContext
        {
            public override void Post(SendOrPostCallback d, object state)
            {
                d(state);
            }

            public override void Send(SendOrPostCallback d, object state)
            {
                d(state);
            }
        }
    }
}