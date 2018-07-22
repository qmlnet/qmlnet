using System;
using System.Threading.Tasks;
using FluentAssertions;
using Qml.Net.Internal.Types;
using Xunit;

namespace Qml.Net.Tests.Types
{
    public class NetDelegateTests
    {
        [Fact]
        public void Can_create_delegate()
        {
            var invoked = false;
            var test = new Action(() => { invoked = true; });
            using (var del = NetDelegate.FromDelegate(test))
            {
                del.Delegate.DynamicInvoke();
            }
            invoked.Should().BeTrue();
        }
    }
}