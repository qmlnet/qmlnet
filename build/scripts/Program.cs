using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static Bullseye.Targets;
using static Build.Buildary.Directory;
using static Build.Buildary.Path;
using static Build.Buildary.Shell;
using static Build.Buildary.Runner;
using static Build.Buildary.Runtime;
using static Build.Buildary.Log;
using static Build.Buildary.File;
using static Build.Buildary.GitVersion;

namespace Build
{
    static class Program
    {
        static Task Main(string[] args)
        {
            var options = ParseOptions<Options>(args);
            
            var nugetApiKey = Environment.GetEnvironmentVariable("PRIVATE_NUGET_KEY");
            var nugetSource = "https://www.myget.org/F/qmlnet/api/v3/index.json";
            var gitversion = GetGitVersion(ExpandPath("./"));
            var commandBuildArgs = $"--configuration {options.Configuration} /p:Platform=\"Any CPU\"";
            var commandBuildArgsWithVersion = commandBuildArgs;
            if (!string.IsNullOrEmpty(gitversion.PreReleaseTag))
            {
                commandBuildArgsWithVersion += $" --version-suffix \"{gitversion.PreReleaseTag}\"";
            }
            
            Info($"Version: {JsonConvert.SerializeObject(gitversion)}");
            
            Target("clean", () =>
            {
                CleanDirectory(ExpandPath("./output"));
            });
            
            Target("test", () =>
            {
                if (IsOSX())
                {
                    // OSX prevent's DYLD_LIBRARY_PATH from being sent to
                    // child shells. We must manually send it.
                    var ldLibraryPath = Environment.GetEnvironmentVariable("DYLD_LIBRARY_PATH");
                    RunShell($"DYLD_LIBRARY_PATH={ldLibraryPath} dotnet test src/net/Qml.Net.Tests/");
                }
                else
                {
                    RunShell($"dotnet test src/net/Qml.Net.Tests/ {commandBuildArgs}");
                }
            });

            Target("build-native", () =>
            {
                if (IsWindows())
                {
                    RunShell($"{ExpandPath("./src/native/build.bat")}");
                }
                else
                {
                    RunShell($"{ExpandPath("./src/native/build.sh")}");
                }
            });

            Target("build-net", () =>
            {
                RunShell($"dotnet build {ExpandPath("src/net/Qml.Net.sln")} {commandBuildArgsWithVersion}");
            });

            Target("build", DependsOn("build-native", "build-net"));

            Target("deploy", DependsOn("clean"), () =>
            {
                // Deploy our nuget packages.
                RunShell($"dotnet pack {ExpandPath("src/net/Qml.Net.sln")} --output {ExpandPath("./output")} {commandBuildArgsWithVersion}");
                if (IsWindows())
                {
                    // Deploy our Windows binaries NuGet package.
                    RunShell($"dotnet pack {ExpandPath("src/native/nuget/Qml.Net.WindowsBinaries.csproj")} --output {ExpandPath("./output")} {commandBuildArgsWithVersion}");
                }
                if (IsOSX())
                {
                    // Deploy our OSX binaries NuGet package.
                    RunShell($"dotnet pack {ExpandPath("src/native/nuget/Qml.Net.OSXBinaries.csproj")} --output {ExpandPath("./output")} {commandBuildArgsWithVersion}");
                }
                if (IsLinux())
                {
                    // Deploy our Linux binaries NuGet package.
                    RunShell($"dotnet pack {ExpandPath("src/native/nuget/Qml.Net.LinuxBinaries.csproj")} --output {ExpandPath("./output")} {commandBuildArgsWithVersion}");
                }
            });
            
            Target("update-version", () =>
            {
                if (FileExists("./build/version.props"))
                {
                    DeleteFile("./build/version.props");
                }
                
                WriteFile("./build/version.props",
                    $@"<Project>
    <PropertyGroup>
        <VersionPrefix>{gitversion.Version}</VersionPrefix>
    </PropertyGroup>
</Project>");
            });
            
            Target("publish", () =>
            {
                if (string.IsNullOrEmpty(nugetApiKey))
                {
                    Info("Skipping publish, due to missing NuGet key...");
                    return;
                }
                void Deploy(string package)
                {
                    RunShell($"dotnet nuget push -k {nugetApiKey} -s {nugetSource} {package}");
                }
                if (IsWindows())
                {
                    Deploy($"./output/Qml.Net.WindowsBinaries.{gitversion.FullVersion}.nupkg");
                }
                if (IsOSX())
                {
                    Deploy($"./output/Qml.Net.OSXBinaries.{gitversion.FullVersion}.nupkg");
                }
                if (IsLinux())
                {
                    Deploy($"./output/Qml.Net.{gitversion.FullVersion}.nupkg");
                    Deploy($"./output/Qml.Net.LinuxBinaries.{gitversion.FullVersion}.nupkg");
                }
            });
            
            Target("default", DependsOn("clean", "build"));

            Target("ci", DependsOn("update-version", "build", "test", "deploy", "publish"));

            return Run(options);
        }

        // ReSharper disable ClassNeverInstantiated.Local
        class Options : RunnerOptions
        // ReSharper restore ClassNeverInstantiated.Local
        {
            [PowerArgs.ArgShortcut("config"), PowerArgs.ArgDefaultValue("Release")]
            public string Configuration { get; set; }
        }
    }
}
