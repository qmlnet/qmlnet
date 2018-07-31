using System.Threading.Tasks;
using static Bullseye.Targets;
using static Build.Buildary.Directory;
using static Build.Buildary.Path;
using static Build.Buildary.Shell;
using static Build.Buildary.Runner;
using static Build.Buildary.Runtime;
using static Build.Buildary.Log;
using static Build.Buildary.File;

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

                    // The windows build currently brings in all the .dll's for packaging.
                    // However, it also brings in the *d.dll/*.pdb files. Let's remove them.
                    foreach(var file in GetFiles(ExpandPath("./src/native/output/"), recursive: true))
                    {
                        if (file.EndsWith("d.dll"))
                        {
                            if(FileExists(file.Substring(0, file.Length - 5) + ".dll"))
                            {
                                // This is a debug dll.
                                DeleteFile(file);
                            }
                        }
                        else if (file.EndsWith(".pdb"))
                        {
                            DeleteFile(file);
                        }
                        else if (file.EndsWith("*.qmlc"))
                        {
                            DeleteFile(file);
                        }
                    }
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
                if (IsWindows())
                {
                    // Deploy our windows Binaries NuGet package.
                    RunShell($"dotnet pack {ExpandPath("src/native/Qml.Net.WindowsBinaries.csproj")} --output {ExpandPath("./output")} {commandBuildArgs}");
                }
            });
            
            Add("default", DependsOn("clean", "build"));

            Add("ci", DependsOn("build", "test", "deploy"));

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