using System;
using System.IO;

namespace Qt.NetCore
{
    public class Helpers
    {
        private static bool _debugVariablesLoaded;
        private static readonly object DebugVariablesLoadedLock = new object();
        public static void LoadDebugVariables()
        {
            if (_debugVariablesLoaded) return;
            lock (DebugVariablesLoadedLock)
            {
                if (_debugVariablesLoaded) return;
                _debugVariablesLoaded = true;
                var filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "debug-variables.txt");
                if(!File.Exists(filePath))
                    throw new Exception("debug-variables.txt doesn't exist in output.");
                var content = File.ReadAllText(filePath);
                if(string.IsNullOrEmpty(content))
                    throw new Exception("No content exists in debug-variables.txt, did you run QtNetCoreQml.pro?");
                string binDir = null;
                using (var reader = new StringReader(content))
                    while (reader.Peek() > 0)
                    {
                        var line = reader.ReadLine();
                        if (string.IsNullOrEmpty(line)) continue;
                        if (line.StartsWith("bin-dir: "))
                            binDir = line.Substring(9);
                    }
                if(string.IsNullOrEmpty(binDir))
                    throw new Exception("No bin-dir specified in debug-variables.txt");
                binDir= binDir.Replace("/", Path.DirectorySeparatorChar.ToString()).Replace("\\", Path.DirectorySeparatorChar.ToString());
                Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + binDir);
            }
        }
    }
}
