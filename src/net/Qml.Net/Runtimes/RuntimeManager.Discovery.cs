using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Qml.Net.Runtimes
{
    public static partial class RuntimeManager
    {
        [Flags]
        public enum RuntimeSearchLocation
        {
            UserDirectory = 1,
            ExecutableDirectory = 1 << 1,
            CurrentDirectory = 1 << 2,
            All = UserDirectory | ExecutableDirectory | CurrentDirectory
        }

        internal static Func<string> GetRuntimeUserDirectory = () =>
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return Path.Combine(Environment.GetEnvironmentVariable("HOME"), ".qmlnet-qt-runtimes");
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return Path.Combine(Environment.GetEnvironmentVariable("%HOMEDRIVE%%HOMEPATH%"), ".qmlnet-qt-runtimes");
            }

            throw new Exception("Unknown platform, can't get user runtimes directory");
        };

        internal static Func<string> GetRuntimeExecutableDirectory = () => Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName);

        internal static Func<string> GetRuntimeCurrentDirectory = () => Path.Combine(Directory.GetCurrentDirectory(), "qmlnet-qt-runtimes");

        public static string[] GetPotentialRuntimesDirectories(RuntimeSearchLocation runtimes = RuntimeSearchLocation.All)
        {
            var result = new List<string>();

            // Order matters, most important location first.
            if (runtimes.HasFlag(RuntimeSearchLocation.ExecutableDirectory))
            {
                result.Add(Path.Combine(GetRuntimeExecutableDirectory()));
            }

            if (runtimes.HasFlag(RuntimeSearchLocation.UserDirectory))
            {
                result.Add(GetRuntimeUserDirectory());
            }

            if (runtimes.HasFlag(RuntimeSearchLocation.CurrentDirectory))
            {
                result.Add(GetRuntimeCurrentDirectory());
            }

            return result.ToArray();
        }

        public static string FindQtRuntime(string[] runtimeDirectories, string qtVersion, RuntimeTarget target)
        {
            if (runtimeDirectories.Length == 0)
            {
                return null;
            }

            var expectedVersion = $"{qtVersion}-{RuntimeTargetToString(target)}";

            foreach (var potentialRuntimeDirectory in runtimeDirectories)
            {
                if (!Directory.Exists(potentialRuntimeDirectory))
                {
                    // Obviously nothing here...
                    continue;
                }

                // Maybe this directory contains a Qt runtime?
                var versionPath = Path.Combine(potentialRuntimeDirectory, "version.txt");

                if (File.Exists(versionPath))
                {
                    var version = File.ReadAllText(versionPath).Trim(Environment.NewLine.ToCharArray());
                    if (version == expectedVersion)
                    {
                        // Found it!
                        return potentialRuntimeDirectory;
                    }
                }

                // Maybe there are nested directories of runtimes?
                var nestedDirectory = Path.Combine(potentialRuntimeDirectory, expectedVersion);
                if (Directory.Exists(nestedDirectory))
                {
                    versionPath = Path.Combine(nestedDirectory, "version.txt");
                    if (File.Exists(versionPath))
                    {
                        var version = File.ReadAllText(versionPath).Trim(Environment.NewLine.ToCharArray());
                        if (version == expectedVersion)
                        {
                            // Found it!
                            return nestedDirectory;
                        }
                    }
                }
            }

            return null;
        }
    }
}