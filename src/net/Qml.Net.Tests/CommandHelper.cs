using System.IO;
using System.Runtime.InteropServices;

namespace Qml.Net.Tests
{
    public static class CommandHelper
    {
        public static void Run(string command, string workingDirectory)
        {
            if (string.IsNullOrEmpty(workingDirectory))
            {
                workingDirectory = Directory.GetCurrentDirectory();
            }
            
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                SimpleExec.Command.Run("cmd.exe", $"/S /C \"{command}\"", workingDirectory, true);
            }

            var escapedArgs = command.Replace("\"", "\\\"");
            SimpleExec.Command.Run("/usr/bin/env", $"bash -c \"{escapedArgs}\"", workingDirectory, true);
        }
    }
}