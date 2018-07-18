using System;
using System.Threading;

namespace Qt.NetCore.Tests
{
    public abstract class BaseTests : IDisposable
    {
        static readonly object LockObject = new object();

        protected BaseTests()
        {
            Monitor.Enter(LockObject);
        }
        
        public virtual void Dispose()
        {
            Monitor.Exit(LockObject);
        }
    }
}