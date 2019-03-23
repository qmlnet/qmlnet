using System;
using System.Threading;

namespace Qml.Net.Tests
{
    public abstract class BaseTests : IDisposable
    {
        static readonly object LockObject = new object();

        protected BaseTests()
        {
            QmlNetConfig.ShouldEnsureUIThread = false; // Not need for unit tests.
            Monitor.Enter(LockObject);
        }

        public virtual void Dispose()
        {
            Monitor.Exit(LockObject);
        }
    }
}