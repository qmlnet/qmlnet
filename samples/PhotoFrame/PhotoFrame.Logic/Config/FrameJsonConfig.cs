using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace PhotoFrame.Logic.Config
{
    // ReSharper disable once ClassNeverInstantiated.Global
    public class FrameConfig
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        // ReSharper disable CollectionNeverUpdated.Global
        public List<string> BorderColors { get; set; }
        public uint BorderWidth { get; set; }
        public uint PhotoShowTimeSeconds { get; set; }
        public string PhotosPath { get; set; }
        public bool ShowDebugInfo { get; set; }
        // ReSharper restore CollectionNeverUpdated.Global
        // ReSharper restore UnusedAutoPropertyAccessor.Global
    }

    public class FrameJsonConfig : IFrameConfig
    {
        public event EventHandler<FrameConfigChangedEventArgs> FrameConfigChanged;

        public List<Color> BorderColors
        {
            get
            {
                return _rawConfig
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

        public uint BorderWidth => _rawConfig.BorderWidth;

        public TimeSpan PhotoShowTime => TimeSpan.FromSeconds(_rawConfig.PhotoShowTimeSeconds);

        public string PhotosPath
        {
            get
            {
                var rawPath = _rawConfig.PhotosPath;
                rawPath = Path.DirectorySeparatorChar == '/' ? rawPath.Replace('\\', '/') : rawPath.Replace('/', '\\');
                if (Path.IsPathRooted(rawPath))
                {
                    return rawPath;
                }
                var wd = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                if(wd == null)
                    throw new NullReferenceException();
                return Path.Combine(wd, rawPath);
            }
        }

        public bool ShowDebugInfo => _rawConfig.ShowDebugInfo;

        private readonly string _jsonFilePath;
        private FrameConfig _rawConfig;
        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly FileSystemWatcher _configWatcher;

        public FrameJsonConfig(string jsonFilePath)
        {
            _jsonFilePath = jsonFilePath;

            LoadConfig();

            var directory = Path.GetDirectoryName(_jsonFilePath);
            if(directory == null)
                throw new NullReferenceException();
            
            _configWatcher = new FileSystemWatcher(directory);
            _configWatcher.Changed += (s, e) =>
            {
                if (string.Equals(e.FullPath, _jsonFilePath))
                {
                    LoadConfig();
                }
            };
            _configWatcher.NotifyFilter = NotifyFilters.LastWrite;
            _configWatcher.EnableRaisingEvents = true;
        }

        private void LoadConfig()
        {
            int tries = 10;
            while (tries > 0)
            {
                try
                {
                    string json;
                    using (var reader = new StreamReader(_jsonFilePath))
                    {
                        json = reader.ReadToEnd();
                    }
                    _rawConfig = JsonConvert.DeserializeObject<FrameConfig>(json);
                    RaiseFrameConfigChanged();
                    break;
                }
                catch(Exception)
                {
                    tries--;
                    if(tries <= 0)
                    {
                        throw;
                    }
                    Thread.Sleep(100);
                }
            }
        }

        private void RaiseFrameConfigChanged()
        {
            FrameConfigChanged?.Invoke(this, new FrameConfigChangedEventArgs());
        }
    }
}
