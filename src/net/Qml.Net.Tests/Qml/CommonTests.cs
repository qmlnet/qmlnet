using System;
using FluentAssertions;
using Xunit;

namespace Qml.Net.Tests.Qml
{
    public class CommonTests
    {
        [Fact]
        public void Can_put_and_get_env()
        {
            var envName = Guid.NewGuid().ToString().Replace("-", "");
            Qt.GetEnv(envName).Should().BeNull();
            Qt.PutEnv(envName, "");
            Qt.GetEnv(envName).Should().BeEmpty();
            Qt.PutEnv(envName, "TEST");
            Qt.GetEnv(envName).Should().Be("TEST");
            Qt.PutEnv(envName, null);
            Qt.GetEnv(envName).Should().BeNull();
        }

        [Fact]
        public void Can_get_qt_version()
        {
            var version = Qt.GetQtVersion();
            version.Major.Should().Be(5);
        }
    }
}