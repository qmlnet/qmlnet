using System.Threading.Tasks;
using static Bullseye.Targets;
using static Build.Buildary.Directory;
using static Build.Buildary.Path;
using static Build.Buildary.Shell;
using static Build.Buildary.Runner;

namespace Build
{
    static class Program
    {
        static Task<int> Main(string[] args)
        {
            var options = ParseOptions<Options>(args);
            
            var commandBuildArgs = $"--configuration {options.Configuration}";

            Add("clean", () =>
            {
                CleanDirectory(ExpandPath("./output"));
            });
            
            Add("test", () =>
            {
                RunShell($"dotnet test src/net/Qml.Net.Tests/ {commandBuildArgs}");
            });

            Add("build", () =>
            {
                // Build the native stuff
                RunShell("./src/native/build.sh");
                // Build the .NETs stuff
                RunShell($"dotnet build src/net/Qml.Net.sln {commandBuildArgs}");
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