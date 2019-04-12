using System;
using System.IO;
using System.Runtime.InteropServices;
using FluentAssertions;
using Qml.Net.Runtimes;
using SharpCompress.Common;
using SharpCompress.Readers;
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

            RuntimeManager.ExtractTarGZStream = (stream, directory) =>
            {
                using (var reader = ReaderFactory.Open(stream, new ReaderOptions()))
                {
                    while (reader.MoveToNextEntry())
                    {
                        if (!reader.Entry.IsDirectory)
                        {
                            reader.WriteEntryToDirectory(directory, new ExtractionOptions()
                            {
                                ExtractFullPath = true,
                                Overwrite = true,
                                WriteSymbolicLink = (sourcePath, targetPath) =>
                                {
                                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                                    {
                                        throw new Exception("File links aren't supported.");
                                    }

                                    var link = new Mono.Unix.UnixSymbolicLinkInfo(sourcePath);
                                    if (File.Exists(sourcePath))
                                    {
                                        link.Delete(); // equivalent to ln -s -f
                                    }

                                    link.CreateSymbolicLinkTo(targetPath);
                                }
                            });
                        }
                    }
                }
            };
        }

        [Fact]
        public void Can_download_runtime()
        {
            RuntimeManager.DownloadRuntimeToDirectory(QmlNetConfig.QtBuildVersion, RuntimeTarget.Windows64, _tempDirectory);
            File.ReadAllText(Path.Combine(_tempDirectory, "version.txt")).Should().Be($"{QmlNetConfig.QtBuildVersion}-win-x64");
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