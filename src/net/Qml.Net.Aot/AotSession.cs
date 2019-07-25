using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

namespace Qml.Net.Aot
{
    public class AotSession : IDisposable
    {
        private readonly string _name;
        private readonly List<AotClass> _classes = new List<AotClass>();

        public AotSession(string name = "interop")
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentOutOfRangeException(nameof(name));
            }

            _name = name;
        }
        
        public ClassMapper<T> MapClass<T>()
        {
            var cls = new AotClass(typeof(T));
            _classes.Add(cls);
            return new ClassMapper<T>(cls);
        }
        
        public ReadOnlyCollection<AotClass> Classes => new ReadOnlyCollection<AotClass>(_classes);

        public void WriteNativeCode(string directory)
        {
            if (string.IsNullOrEmpty(directory))
            {
                throw new ArgumentException(nameof(directory));
            }
            
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
            else
            {
                if (Directory.GetFiles(directory).Length > 0 || Directory.GetDirectories(directory).Length > 0)
                {
                    throw new Exception($"The directory {directory} is not empty.");
                }
            }

            var priFile = Path.Combine(directory, $"{_name}-native.pri");
            using (var stream = File.OpenWrite(priFile))
            {
                using (var writer = new StreamWriter(stream))
                {
                    writer.WriteLine("INCLUDEPATH += $$PWD");
                    var headers = new StringBuilder();
                    var sources = new StringBuilder();

                    if (_classes.Count > 0)
                    {
                        for (var x = 0; x < _classes.Count; x++)
                        {
                            foreach (var cls in _classes)
                            {
                                headers.AppendLine($"    {cls.Type.Name}.h \\");
                                sources.AppendLine($"    {cls.Type.Name}.cpp \\");
                            }
                        }

                        writer.WriteLine("HEADERS += \\");
                        writer.Write(headers);

                        writer.WriteLine("SOURCES += \\");
                        writer.Write(sources);
                    }
                }
            }
        }
        
        public void Dispose()
        {
        }
    }
}