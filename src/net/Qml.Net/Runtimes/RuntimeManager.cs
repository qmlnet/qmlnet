using System;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Threading;

namespace Qml.Net.Runtimes
{
    public static partial class RuntimeManager
    {
        public delegate string BuildRuntimeUrlDelegate(string qtVersion, RuntimeTarget target);

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static BuildRuntimeUrlDelegate BuildRuntimeUrl = (qtVersion, target)
            => $"https://github.com/qmlnet/qt-runtimes/releases/download/releases/{qtVersion}-{RuntimeTargetToString(target)}-runtime.tar.gz";

        private static string RuntimeTargetToString(RuntimeTarget target)
        {
            switch (target)
            {
                case RuntimeTarget.Windows64:
                    return "win-x64";
                case RuntimeTarget.LinuxX64:
                    return "linux-x64";
                case RuntimeTarget.OSX64:
                    return "osx-x64";
                case RuntimeTarget.Unsupported:
                    throw new Exception("Unsupported target");
                default:
                    throw new Exception($"Unknown target {target}");
            }
        }

        public delegate void ExtractTarGZStreamDelegate(Stream stream, string destinationDirectory);

        // ReSharper disable once MemberCanBePrivate.Global
        // ReSharper disable once FieldCanBeMadeReadOnly.Global
        public static ExtractTarGZStreamDelegate ExtractTarGZStream = Tar.ExtractTarFromGzipStream;

        // ReSharper disable once MemberCanBePrivate.Global
        public static RuntimeTarget GetCurrentRuntimeTarget()
        {
            if (IntPtr.Size != 8)
            {
                return RuntimeTarget.Unsupported;
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

            return RuntimeTarget.Unsupported;
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

        private static void GetUrlStream(string url, Action<Stream> action)
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

        public static string FindSuitableQtRuntime(RuntimeSearchLocation runtimeSearchLocation = RuntimeSearchLocation.All)
        {
            var potentials = GetPotentialRuntimesDirectories(runtimeSearchLocation);
            return FindQtRuntime(potentials, QmlNetConfig.QtBuildVersion, GetCurrentRuntimeTarget());
        }

        public static void DiscoverOrDownloadSuitableQtRuntime(RuntimeSearchLocation runtimeSearchLocation = RuntimeSearchLocation.All)
        {
            var suitableRuntime = FindSuitableQtRuntime(runtimeSearchLocation);
            if (!string.IsNullOrEmpty(suitableRuntime))
            {
                // Found one!
                ConfigureRuntimeDirectory(suitableRuntime);
                return;
            }

            var currentTarget = GetCurrentRuntimeTarget();
            var version = $"{QmlNetConfig.QtBuildVersion}-{RuntimeTargetToString(currentTarget)}";

            // Let's try to download and install the Qt runtime into the users directory.
            var destinationDirectory = Path.Combine(GetPotentialRuntimesDirectories(RuntimeSearchLocation.UserDirectory).Single(), version);
            var destinationTmpDirectory = $"{destinationDirectory}-{Guid.NewGuid().ToString().Replace("-", "")}";

            Directory.CreateDirectory(destinationTmpDirectory);
            DownloadRuntimeToDirectory(QmlNetConfig.QtBuildVersion, currentTarget, destinationTmpDirectory);

            if (Directory.Exists(destinationDirectory))
            {
                Directory.Delete(destinationDirectory, true);
            }

            Directory.Move(destinationTmpDirectory, destinationDirectory);

            ConfigureRuntimeDirectory(destinationDirectory);
        }
    }
}