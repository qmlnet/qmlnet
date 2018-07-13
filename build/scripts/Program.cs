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
            
            Add("build", () =>
            {
                RunShell($"dotnet build src/net/Qt.NetCore.sln {commandBuildArgs}");
            });
            
            Add("swig-run", () =>
            {
                var swigCommand = "swig3.0 -csharp -c++ " +
                    "-namespace Qt.NetCore " +
                    "-outfile Interop.cs " +
                    "-outdir src/interop " +
                    "-o src/native/QtNetCoreQml/swig.cpp " +
                    "-oh src/native/QtNetCoreQml/swig.h " +
                    "src/native/QtNetCoreQml/swig/QtNetCoreQml.i";
                RunShell($"docker run -it -v {CurrentDirectory()}:/work net-core-qml-swig {swigCommand}");
            });
            
            Add("swig-build", () =>
            {
                RunShell("docker build ./build/docker -f ./build/docker/Dockerfile.swig -t net-core-qml-swig");
            });
            
            Add("default", DependsOn("clean", "build"));

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