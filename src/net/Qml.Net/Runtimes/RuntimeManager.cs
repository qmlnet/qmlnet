using System;
using System.IO;
using System.Linq;
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
                default:
                    throw new Exception($"Unknown target {target}");
            }
        }

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
            var expectedVersion = $"{QmlNetConfig.QtBuildVersion}-{RuntimeTargetToString(GetCurrentRuntimeTarget())}";

            if (version != expectedVersion)
            {
                throw new Exception($"The version of the runtime directory was {versionFile}, but expected {expectedVersion}");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var pluginsDirectory = Path.Combine(directory, "qt", "plugins");
                if (!Directory.Exists(pluginsDirectory))
                {
                    throw new Exception($"Plugins directory didn't exist: {pluginsDirectory}");
                }
                Environment.SetEnvironmentVariable("QT_PLUGIN_PATH", pluginsDirectory);

                var qmlDirectory = Path.Combine(directory, "qt", "qml");
                if (!Directory.Exists(qmlDirectory))
                {
                    throw new Exception($"QML directory didn't exist: {qmlDirectory}");
                }
                Environment.SetEnvironmentVariable("QML2_IMPORT_PATH", qmlDirectory);

                var libDirectory = Path.Combine(directory, "qt", "lib");
                if (!Directory.Exists(libDirectory))
                {
                    throw new Exception($"The lib directory didn't exist: {libDirectory}");
                }

                var preloadPath = Path.Combine(libDirectory, "preload.txt");
                if (!File.Exists(preloadPath))
                {
                    throw new Exception($"The preload.txt file didn't exist: {preloadPath}");
                }

                var libsToPreload = File.ReadAllLines(preloadPath).Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => Path.Combine(libDirectory, x))
                    .ToList();
                var platformLoader = NetNativeLibLoader.Loader.PlatformLoaderBase.SelectPlatformLoader();
                foreach (var libToPreload in libsToPreload)
                {
                    var libHandler = platformLoader.LoadLibrary(libToPreload);
                    if (libHandler == IntPtr.Zero)
                    {
                        throw new Exception($"Unabled to preload library: {libToPreload}");
                    }
                }
            }
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var pluginsDirectory = Path.Combine(directory, "qt", "plugins");
                if (!Directory.Exists(pluginsDirectory))
                {
                    throw new Exception($"Plugins directory didn't exist: {pluginsDirectory}");
                }
                Environment.SetEnvironmentVariable("QT_PLUGIN_PATH", pluginsDirectory);

                var qmlDirectory = Path.Combine(directory, "qt", "qml");
                if (!Directory.Exists(qmlDirectory))
                {
                    throw new Exception($"QML directory didn't exist: {qmlDirectory}");
                }
                Environment.SetEnvironmentVariable("QML2_IMPORT_PATH", qmlDirectory);

                var libDirectory = Path.Combine(directory, "qt", "lib");
                if (!Directory.Exists(libDirectory))
                {
                    throw new Exception($"The lib directory didn't exist: {libDirectory}");
                }

                var preloadPath = Path.Combine(libDirectory, "preload.txt");
                if (!File.Exists(preloadPath))
                {
                    throw new Exception($"The preload.txt file didn't exist: {preloadPath}");
                }

                var libsToPreload = File.ReadAllLines(preloadPath).Where(x => !string.IsNullOrEmpty(x))
                    .Select(x => Path.Combine(libDirectory, x))
                    .ToList();
                var platformLoader = NetNativeLibLoader.Loader.PlatformLoaderBase.SelectPlatformLoader();
                foreach (var libToPreload in libsToPreload)
                {
                    var libHandler = platformLoader.LoadLibrary(libToPreload);
                    if (libHandler == IntPtr.Zero)
                    {
                        throw new Exception($"Unabled to preload library: {libToPreload}");
                    }
                }
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var pluginsDirectory = Path.Combine(directory, "qt", "plugins");
                if (!Directory.Exists(pluginsDirectory))
                {
                    throw new Exception($"Plugins directory didn't exist: {pluginsDirectory}");
                }
                Environment.SetEnvironmentVariable("QT_PLUGIN_PATH", pluginsDirectory);

                var qmlDirectory = Path.Combine(directory, "qt", "qml");
                if (!Directory.Exists(qmlDirectory))
                {
                    throw new Exception($"QML directory didn't exist: {qmlDirectory}");
                }
                Environment.SetEnvironmentVariable("QML2_IMPORT_PATH", qmlDirectory);

                var binDirectory = Path.Combine(directory, "qt", "bin");
                if (!Directory.Exists(binDirectory))
                {
                    throw new Exception($"The bin directory didn't exist: {binDirectory}");
                }

                Environment.SetEnvironmentVariable("PATH", $"{binDirectory};{Environment.GetEnvironmentVariable("PATH")}");

                var preloadPath = Path.Combine(binDirectory, "preload.txt");
                if (!File.Exists(preloadPath))
                {
                    throw new Exception($"The preload.txt file didn't exist: {preloadPath}");
                }
            }
        }
    }
}