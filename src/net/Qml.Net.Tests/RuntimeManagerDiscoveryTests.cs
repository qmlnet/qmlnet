using System.IO;
using System.Linq;
using FluentAssertions;
using Qml.Net.Runtimes;
using Xunit;

namespace Qml.Net.Tests
{
    public class RuntimeManagerDiscoveryTests : BaseRuntimeManagerTests
    {
        [Theory]
        [InlineData(RuntimeManager.RuntimeSearchLocation.UserDirectory)]
        public void Can_find_qt_runtimes(RuntimeManager.RuntimeSearchLocation runtimeSearchLocation)
        {
            var directory = Path.Combine(RuntimeManager.GetPotentialRuntimesDirectories(runtimeSearchLocation).Single());

            var found = RuntimeManager.FindQtRuntime(
                RuntimeManager.GetPotentialRuntimesDirectories(runtimeSearchLocation),
                "qt-version",
                RuntimeTarget.Windows64);

            found.Should().BeNullOrEmpty();

            File.WriteAllText(Path.Combine(directory, "version.txt"), "qt-version-win-x64");

            found = RuntimeManager.FindQtRuntime(
                RuntimeManager.GetPotentialRuntimesDirectories(runtimeSearchLocation),
                "qt-version",
                RuntimeTarget.Windows64);

            found.Should().Be(directory);

            File.Delete(Path.Combine(directory, "version.txt"));

            var nestedRuntimeDirectory = Path.Combine(directory, "qt-version-win-x64");
            Directory.CreateDirectory(nestedRuntimeDirectory);

            File.WriteAllText(Path.Combine(nestedRuntimeDirectory, "version.txt"), "qt-version-win-x64");

            found = RuntimeManager.FindQtRuntime(
                RuntimeManager.GetPotentialRuntimesDirectories(runtimeSearchLocation),
                "qt-version",
                RuntimeTarget.Windows64);

            found.Should().Be(nestedRuntimeDirectory);
        }

        [Fact]
        public void Can_find_runtimes_in_proper_order()
        {
            File.WriteAllText(Path.Combine(_runtimeCurrentDirectory, "version.txt"), "qt-version-win-x64");

            var found = RuntimeManager.FindQtRuntime(
                RuntimeManager.GetPotentialRuntimesDirectories(),
                "qt-version",
                RuntimeTarget.Windows64);

            found.Should().Be(_runtimeCurrentDirectory);

            File.WriteAllText(Path.Combine(_runtimeUserDirectory, "version.txt"), "qt-version-win-x64");

            found = RuntimeManager.FindQtRuntime(
                RuntimeManager.GetPotentialRuntimesDirectories(),
                "qt-version",
                RuntimeTarget.Windows64);

            found.Should().Be(_runtimeUserDirectory);

            File.WriteAllText(Path.Combine(_runtimeExecutableDirectory, "version.txt"), "qt-version-win-x64");

            found = RuntimeManager.FindQtRuntime(
                RuntimeManager.GetPotentialRuntimesDirectories(),
                "qt-version",
                RuntimeTarget.Windows64);

            found.Should().Be(_runtimeExecutableDirectory);
        }
    }
}