using System.Threading.Tasks;
using static Bullseye.Targets;
using static Build.Buildary.Directory;
using static Build.Buildary.Path;
using static Build.Buildary.Shell;
using static Build.Buildary.Runner;
using static Build.Buildary.Runtime;

namespace Build
{
    static class Program
    {
        static Task<int> Main(string[] args)
        {
            var options = ParseOptions<Options>(args);
            
            var commandBuildArgs = $"--configuration {options.Configuration} /p:Platform=\"Any CPU\"";

            Add("clean", () =>
            {
                CleanDirectory(ExpandPath("./output"));
            });
            
            Add("test", () =>
            {
                RunShell($"dotnet test src/net/Qml.Net.Tests/ {commandBuildArgs}");
            });

            Add("build-native", () =>
            {
                if (IsWindows())
                {
                    RunShell($"{ExpandPath("./src/native/build.bat")}");
                }
                else
                {
                    RunShell("src/native/build.sh");
                }
            });

            Add("build-net", () =>
            {
                RunShell($"dotnet build {ExpandPath("src/net/Qml.Net.sln")} {commandBuildArgs}");
            });

            Add("build", DependsOn("build-native", "build-net"));

            Add("deploy", DependsOn("clean"), () =>
            {
                // Deploy our nuget packages.
                RunShell($"dotnet pack {ExpandPath("src/net/Qml.Net.sln")} --output {ExpandPath("./output")} {commandBuildArgs}");
            });
            
            Add("default", DependsOn("clean", "build"));

            Add("ci", DependsOn("build", "test"));

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