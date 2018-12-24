using System;
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
                return Task.FromResult(string.Empty);
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

                RunQmlTest(
                    "test",
                    @"
                        var task = test.testAsync()
                        Net.await(task, function() {
                            test.testMethod()
                        })
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

                RunQmlTest(
                    "test",
                    @"
                        var task = test.testAsync()
                        Net.await(task, function(result) {
                            test.testMethodWithArg(result)
                        })
                    ");

                Mock.Verify(x => x.TestMethodWithArg("testt"), Times.Once);
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(oldContext);
            }
        }

        [Fact]
        public void Can_listen_to_exception_throw()
        {
            var oldContext = SynchronizationContext.Current;
            SynchronizationContext.SetSynchronizationContext(new NoAsyncSynchronizationContext());

            try
            {
                Mock.Setup(x => x.TestAsync()).Returns(Task.FromException<Exception>(new Exception("throw this")));
                Mock.Setup(x => x.TestMethodWithArg("throw this"));
                Mock.Setup(x => x.TestMethodWithArg("success"));

                RunQmlTest(
                    "test",
                    @"
                        var task = test.testAsync('throw this')
                        Net.await(task, function(result) {
                            test.testMethodWithArg('success')
                        },
                        function(ex) {
                            test.testMethodWithArg(ex.message)
                        })
                    ");

                Mock.Verify(x => x.TestMethodWithArg("throw this"), Times.Once);
                Mock.Verify(x => x.TestMethodWithArg("success"), Times.Never);
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