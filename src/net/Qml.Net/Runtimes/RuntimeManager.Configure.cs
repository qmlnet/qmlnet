using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Qml.Net.Runtimes
{
    public static partial class RuntimeManager
    {
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

                return;
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

                return;
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

                return;
            }

            throw new Exception("Unknown platform, can't configure runtime directory");
        }
    }
}