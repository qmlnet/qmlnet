using System;
using System.IO;
using System.Runtime.InteropServices;
using FluentAssertions;
using Mono.Unix;
using Qml.Net.Runtimes;
using Xunit;

namespace Qml.Net.Tests
{
    public class RuntimeManagerTests : IDisposable
    {
        private readonly string _tempDirectory;

        public RuntimeManagerTests()
        {
            _tempDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString().Replace("-", ""));
            Directory.CreateDirectory(_tempDirectory);
        }

        [Fact]
        public void Can_download_windows_runtime()
        {
            RuntimeManager.DownloadRuntimeToDirectory(QmlNetConfig.QtBuildVersion, RuntimeTarget.Windows64, _tempDirectory);
            File.ReadAllText(Path.Combine(_tempDirectory, "version.txt")).Should().Be($"{QmlNetConfig.QtBuildVersion}-win-x64");
        }

        [Fact]
        public void Can_download_linux_runtime()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                RuntimeManager.DownloadRuntimeToDirectory(QmlNetConfig.QtBuildVersion, RuntimeTarget.LinuxX64, _tempDirectory);
                File.ReadAllText(Path.Combine(_tempDirectory, "version.txt")).Should().Be($"{QmlNetConfig.QtBuildVersion}-linux-x64");

                // Make sure the permissions are set correctly.
                var permissions = UnixFileSystemInfo
                    .GetFileSystemEntry(Path.Combine(_tempDirectory, "qt", "lib", "libQt5Xml.so.5.15.0"))
                    .FileAccessPermissions;
                permissions.Should().Be(FileAccessPermissions.UserReadWriteExecute
                                        | FileAccessPermissions.GroupRead | FileAccessPermissions.GroupExecute
                                        | FileAccessPermissions.OtherRead | FileAccessPermissions.OtherExecute);

                // Make sure links are setup correctly.
                File.Exists(Path.Combine(_tempDirectory, "qt", "lib", "libQt5Xml.so.5")).Should().BeTrue();
            }
        }

        [Fact]
        public void Can_download_osx_runtime()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                RuntimeManager.DownloadRuntimeToDirectory(QmlNetConfig.QtBuildVersion, RuntimeTarget.OSX64, _tempDirectory);
                File.ReadAllText(Path.Combine(_tempDirectory, "version.txt")).Should().Be($"{QmlNetConfig.QtBuildVersion}-osx-x64");

                var permissions = UnixFileInfo
                    .GetFileSystemEntry(Path.Combine(_tempDirectory, "qt", "lib", "QtXml.framework", "Versions", "5", "QtXml"))
                    .FileAccessPermissions;
                permissions.Should().Be(FileAccessPermissions.UserReadWriteExecute
                                        | FileAccessPermissions.GroupRead | FileAccessPermissions.GroupExecute
                                        | FileAccessPermissions.OtherRead | FileAccessPermissions.OtherExecute);

                // Make sure links are setup correctly.
                Directory.Exists(Path.Combine(_tempDirectory, "qt", "lib", "QtXml.framework", "Versions", "Current")).Should().BeTrue();
            }
        }

        public void Dispose()
        {
            if (Directory.Exists(_tempDirectory))
            {
                Directory.Delete(_tempDirectory, true);
            }
        }
    }
}