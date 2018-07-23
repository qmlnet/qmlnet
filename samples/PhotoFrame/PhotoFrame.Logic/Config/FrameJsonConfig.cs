using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;

namespace PhotoFrame.Logic.Config
{
    public class FrameConfig
    {
        public List<string> BorderColors { get; set; }
        public uint BorderWidth { get; set; }
        public uint PhotoShowTimeSeconds { get; set; }
        public string PhotosPath { get; set; }
        public bool ShowDebugInfo { get; set; }
    }

    public class FrameJsonConfig : IFrameConfig
    {
        public event EventHandler<FrameConfigChangedEventArgs> FrameConfigChanged;

        public List<Color> BorderColors
        {
            get
            {
                return _RawConfig
                    .BorderColors
                    .Select(cs =>
                    {
                        var csTrim = cs.Replace("#", "");
                        if (csTrim.Length == 6)
                        {
                            csTrim = "FF" + csTrim.ToUpperInvariant();
                        }
                        else if (csTrim.Length == 8)
                        {
                            csTrim = csTrim.ToUpperInvariant();
                        }
                        var argb = Int32.Parse(csTrim, NumberStyles.HexNumber);
                        return Color.FromArgb(argb);
                    })
                    .ToList();
            }
        }

        public uint BorderWidth
        {
            get
            {
                return _RawConfig.BorderWidth;
            }
        }

        public TimeSpan PhotoShowTime
        {
            get
            {
                return TimeSpan.FromSeconds(_RawConfig.PhotoShowTimeSeconds);
            }
        }

        public string PhotosPath
        {
            get
            {
                var rawPath = _RawConfig.PhotosPath;
                if(Path.DirectorySeparatorChar == '/')
                {
                    rawPath = rawPath.Replace('\\', '/');
                }
                else
                {
                    rawPath = rawPath.Replace('/', '\\');
                }
                if (Path.IsPathRooted(rawPath))
                {
                    return rawPath;
                }
                var wd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                return Path.Combine(wd, rawPath);
            }
        }

        public bool ShowDebugInfo
        {
            get
            {
                return _RawConfig.ShowDebugInfo;
            }
        }

        private string _JsonFilePath;
        private FrameConfig _RawConfig = null;
        private FileSystemWatcher _ConfigWatcher;

        public FrameJsonConfig(string jsonFilePath)
        {
            _JsonFilePath = jsonFilePath;

            LoadConfig();

            var directory = Path.GetDirectoryName(_JsonFilePath);
            var fileName = Path.GetFileName(_JsonFilePath);

            _ConfigWatcher = new FileSystemWatcher(directory);
            _ConfigWatcher.Changed += (s, e) =>
            {
                if (string.Equals(e.FullPath, _JsonFilePath))
                {
                    LoadConfig();
                }
            };
            _ConfigWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _ConfigWatcher.EnableRaisingEvents = true;
        }

        private void LoadConfig()
        {
            string json = "";
            int tries = 10;
            while (tries > 0)
            {
                try
                {
                    using (var reader = new StreamReader(_JsonFilePath))
                    {
                        json = reader.ReadToEnd();
                    }
                    _RawConfig = JsonConvert.DeserializeObject<FrameConfig>(json);
                    RaiseFrameConfigChanged();
                    break;
                }
                catch(Exception e)
                {
                    tries--;
                    if(tries <= 0)
                    {
                        throw e;
                    }
                    Thread.Sleep(100);
                    continue;
                }
            }
        }

        private void RaiseFrameConfigChanged()
        {
            FrameConfigChanged?.Invoke(this, new FrameConfigChangedEventArgs());
        }
    }
}
