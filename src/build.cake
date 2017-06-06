#addin "Cake.Docker"

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
    RunSwigCommand("bash");
});

Task("Default")
    .IsDependentOn("RunSwig");

RunTarget(target);

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
        "sh -c '" + command + "'");

}