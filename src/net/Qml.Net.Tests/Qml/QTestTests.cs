using System;
using FluentAssertions;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class QTestTests : AbstractBaseQmlTests<QTestTests.Dummy>
    {
        public class Dummy
        {
        }

        [Fact]
        public void Can_wait_for()
        {
            int counter = 0;
            QTest.QWaitFor(
                () =>
            {
                counter++;
                return counter == 5;
            }, TimeSpan.FromSeconds(5)).Should().BeTrue();
            counter.Should().Be(5);
        }

        [Fact]
        public void Can_wait_for_timeout()
        {
            QTest.QWaitFor(() => { return false; }, TimeSpan.FromSeconds(1)).Should().BeFalse();
        }
    }
}