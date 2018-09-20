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
            var nugetSource = "https://feeds.pknopf.com/nuget/qmlnet";
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

                    if (IsOSX())
                    {
                        // We deploy the entire Qt framework. Let's trim it down.
                        foreach(var directory in GetDirecories(ExpandPath("./src/native/output"), recursive:true))
                        {
                            if (!DirectoryExists(directory))
                            {
                                continue;
                            }
                            
                            var directoryName = Path.GetFileName(directory);
                            if (directoryName == "Headers")
                            {
                                DeleteDirectory(directory);
                                continue;
                            }
                            
                            if (directoryName.EndsWith(".dSYM"))
                            {
                                DeleteDirectory(directory);
                                continue;
                            }

                            if (directory == "cmake")
                            {
                                DeleteDirectory(directory);
                                continue;
                            }

                            if (directory == "pkgconfig")
                            {
                                DeleteDirectory(directory);
                            }
                        }

                        foreach (var file in GetFiles(ExpandPath("./src/native/output"), recursive:true))
                        {
                            var extension = Path.GetExtension(file);
                            var fileName = Path.GetFileNameWithoutExtension(file);
                            
                            if (fileName.EndsWith("_debug"))
                            {
                                DeleteFile(file);
                                continue;
                            }
                            
                            if (extension == ".prl")
                            {
                                DeleteFile(file);
                                continue;
                            }
                            
                            if (extension == ".plist")
                            {
                                DeleteFile(file);
                                continue;
                            }

                            if (extension == ".qmlc")
                            {
                                DeleteFile(file);
                                continue;
                            }

                            if (extension == ".cmake")
                            {
                                DeleteFile(file);
                                continue;
                            }

                            if (extension == ".a")
                            {
                                DeleteFile(file);
                                continue;
                            }

                            if (extension == ".la")
                            {
                                DeleteFile(file);
                            }
                        }
                    }

                    if (IsLinux())
                    {
                        // First get a list of all dependencies from every .so files.
                        var linkedFiles = new List<string>();
                        foreach(var file in GetFiles(ExpandPath("./src/native/output"), pattern:"*.so*", recursive:true))
                        {
                            var lddOutput = ReadShell($"ldd {file}");
                            foreach (var _line in lddOutput.Split(Environment.NewLine))
                            {
                                var line = _line.TrimStart('\t').TrimStart('\n');
                                var match = Regex.Match(line, @"(.*) =>.*");
                                if (match.Success)
                                {
                                    var linkedFile = match.Groups[1].Value;
                                    if(!linkedFiles.Contains(linkedFile))
                                    {
                                        linkedFiles.Add(linkedFile);
                                    }
                                }
                            }
                        }
                        
                        // Let's remove any file from lib/ that isn't linked against anything.
                        foreach(var file in GetFiles(ExpandPath("./src/native/output/lib"), recursive:true))
                        {
                            var fileName = Path.GetFileName(file);
                            if (!linkedFiles.Contains(fileName))
                            {
                                DeleteFile(file);
                            }
                        }

                        foreach (var directory in GetDirecories(ExpandPath("./src/native/output"), recursive: true))
                        {
                            if (!DirectoryExists(directory))
                            {
                                continue;
                            }
                            
                            var directoryName = Path.GetFileName(directory);
                            if (directoryName == "cmake")
                            {
                                DeleteDirectory(directory);
                                continue;
                            }

                            if (directoryName == "pkgconfig")
                            {
                                DeleteDirectory(directory);
                                continue;
                            }
                            
                            Info(directory);
                        }
                        
                        foreach (var file in GetFiles(ExpandPath("./src/native/output"), recursive: true))
                        {
                            var fileName = Path.GetFileName(file);
                            var fileExtension = Path.GetExtension(fileName);
                            
                            if (fileExtension == ".qmlc")
                            {
                                DeleteFile(file);
                            }
                        }
                    }
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
                    RunShell($"dotnet pack {ExpandPath("src/native/Qml.Net.WindowsBinaries.csproj")} --output {ExpandPath("./output")} {commandBuildArgsWithVersion}");
                }
                if (IsOSX())
                {
                    // Deploy our OSX binaries NuGet package.
                    RunShell($"dotnet pack {ExpandPath("src/native/Qml.Net.OSXBinaries.csproj")} --output {ExpandPath("./output")} {commandBuildArgsWithVersion}");
                }
                if (IsLinux())
                {
                    // Deploy our Linux binaries NuGet package.
                    RunShell($"dotnet pack {ExpandPath("src/native/Qml.Net.LinuxBinaries.csproj")} --output {ExpandPath("./output")} {commandBuildArgsWithVersion}");
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
                return; //temp
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