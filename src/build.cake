#addin "Cake.Docker"
#addin "nuget:?package=Cake.Watch"

var target = Argument("target", "Default");

Task("PrepareSwigContainer")
    .Does(() =>
{
    DockerBuild(new DockerBuildSettings {
            File = "Dockerfile.Swig",
            Tag = new string[] {
                "net-core-qml-swig"
            }
        },
        ".");
});

Task("RunSwig")
    .Does(() =>
{
    RunSwig();
});

Task("RunSwigWatch")
    .Does(() =>
{
    RunSwig();
    var settings = new WatchSettings {
        Recursive = true,
        Path = "./native/QtNetCoreQml/swig/",
        Pattern = "*.i"
    };
    Watch(settings, (changes) => {
        RunSwig();
    });
});

Task("Default")
    .IsDependentOn("RunSwig");

RunTarget(target);

void RunSwig()
{
    Information("Running swig...");
    RunSwigCommand("swig3.0 -csharp -c++ -namespace Qt.NetCore -outfile Interop.cs -outdir interop -o native/QtNetCoreQml/swig.cpp -oh native/QtNetCoreQml/swig.h native/QtNetCoreQml/swig/QtNetCoreQml.i");
}

void RunSwigCommand(string command)
{
    var mountVolume = System.IO.Directory.GetCurrentDirectory() + ":/work";

    DockerRun(
        new DockerRunSettings {
            Volume = new string[] {
                mountVolume
            },
            Workdir = "/work",
            Interactive = true,
            Tty = true
        },
        "net-core-qml-swig",
        command);

}