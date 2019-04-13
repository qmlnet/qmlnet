using System;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;

namespace Qml.Net.Runtimes
{
    public static class RuntimeManager
    {
        public delegate string BuildRuntimeUrlDelegate(string qtVersion, RuntimeTarget target);

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static BuildRuntimeUrlDelegate BuildRuntimeUrl = (qtVersion, target) =>
        {
            var url = $"https://github.com/qmlnet/qt-runtimes/releases/download/releases/{qtVersion}-{{target}}-runtime.tar.gz";
            switch (target)
            {
                case RuntimeTarget.Windows64:
                    return url.Replace("{target}", "win-x64");
                case RuntimeTarget.LinuxX64:
                    return url.Replace("{target}", "linux-x64");
                case RuntimeTarget.OSX64:
                    return url.Replace("{target}", "osx-x64");
                default:
                    throw new Exception($"Unknown target {target}");
            }
        };

        public delegate void ExtractTarGZStreamDelegate(Stream stream, string destinationDirectory);

        public static ExtractTarGZStreamDelegate ExtractTarGZStream = Tar.ExtractTarFromGzipStream;

        public static RuntimeTarget GetCurrentRuntimeTarget()
        {
            if (IntPtr.Size != 8)
            {
                throw new Exception("Only 64bit supported");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return RuntimeTarget.Windows64;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return RuntimeTarget.LinuxX64;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return RuntimeTarget.OSX64;
            }

            throw new Exception("Unknown OS platform");
        }

        public static void DownloadRuntimeToDirectory(
            string qtVersion,
            RuntimeTarget runtimeTarget,
            string destinationDirectory)
        {
            var extractTarGZStreamDel = ExtractTarGZStream;
            if (extractTarGZStreamDel == null)
            {
                throw new Exception("You must set RuntimeManager.ExtractTarGZStream to properly extract a tar file.");
            }

            if (!Directory.Exists(destinationDirectory))
            {
                throw new Exception($"The directory \"{destinationDirectory}\" doesn't exist.");
            }

            if (Directory.GetFiles(destinationDirectory).Length > 0)
            {
                throw new Exception("The directory is not empty");
            }

            if (Directory.GetDirectories(destinationDirectory).Length > 0)
            {
                throw new Exception("The directory is not empty");
            }

            var url = BuildRuntimeUrl(qtVersion, runtimeTarget);

            GetUrlStream(url, stream =>
            {
                extractTarGZStreamDel(stream, destinationDirectory);
            });
        }

        public static void GetUrlStream(string url, Action<Stream> action)
        {
            var syncContext = SynchronizationContext.Current;
            try
            {
                SynchronizationContext.SetSynchronizationContext(null);

                using (var httpClient = new HttpClient())
                {
                    action(httpClient.GetStreamAsync(url).GetAwaiter().GetResult());
                }
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(syncContext);
            }
        }

        public static void ConfigureRuntimeDirectory(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentNullException(nameof(directory));
            }

            if (!Directory.Exists(directory))
            {
                throw new Exception("The directory doesn't exist.");
            }

            var versionFile = Path.Combine(directory, "version.txt");

            if (!File.Exists(versionFile))
            {
                throw new Exception("The version.txt file doesn't exist in the directory.");
            }

            var version = File.ReadAllText(versionFile).TrimEnd(Environment.NewLine.ToCharArray());
            var expectedVersion = $"{QmlNetConfig.QtBuildVersion}-{GetCurrentRuntimeTarget()}";

            if (version != expectedVersion)
            {
                throw new Exception($"The version of the runtime directory was {versionFile}, but expected {expectedVersion}");
            }

            var pluginsDirectory = Path.Combine(directory, "plugins");
            if (!Directory.Exists(pluginsDirectory))
            {
                throw new Exception($"Plugins directory didn't exist: {pluginsDirectory}");
            }
            Environment.SetEnvironmentVariable("QT_PLUGIN_PATH", pluginsDirectory);

            var qmlDirectory = Path.Combine(directory, "qml");
            if (!Directory.Exists(qmlDirectory))
            {
                throw new Exception($"QML directory didn't exist: {qmlDirectory}");
            }
            Environment.SetEnvironmentVariable("QML2_IMPORT_PATH", qmlDirectory);

            /*if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                if (!string.IsNullOrEmpty(libDirectory) && Directory.Exists(libDirectory))
                {
                    // Even though we opened up the native dll correctly, we need to add
                    // the folder to the path. The reason is because QML plugins aren't
                    // in the same directory and have trouble finding dependencies
                    // that are within our lib folder.
                    Environment.SetEnvironmentVariable(
                        "PATH",
                        Environment.GetEnvironmentVariable("PATH") + $";{libDirectory}");
                }
            }*/
        }
    }
}