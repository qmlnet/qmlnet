using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Qml.Net.Aot
{
    public class AotSession : IDisposable
    {
        private readonly AotSessionOptions _options;
        private readonly List<AotClass> _classes = new List<AotClass>();
        private static int _aotTypeIdCounter;
        
        public AotSession(AotSessionOptions options = null)
        {
            if (options == null)
            {
                options = new AotSessionOptions();
            }
            
            if (string.IsNullOrEmpty(options.Name))
            {
                throw new Exception("options.Name is required.");
            }

            if (string.IsNullOrEmpty(options.NetNamespace))
            {
                throw new Exception("options.NetNamespace is required.");
            }

            _options = options;
        }
        
        public ClassMapper<T> MapClass<T>()
        {
            var cls = new AotClass(typeof(T), Interlocked.Increment(ref _aotTypeIdCounter));
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

            if (_classes.All(x => x.Type != typeof(object)))
            {
                _classes.Add(new AotClass(typeof(object), Interlocked.Increment(ref _aotTypeIdCounter)));
            }
            
            var priFile = Path.Combine(directory, $"{_options.Name}.pri");
            using (var writer = new CodeWriter(priFile))
            {
                writer.WriteLine("INCLUDEPATH += $$PWD");
                
                writer.WriteLine("HEADERS += \\");
                using (writer.BeginIndent())
                {
                    writer.WriteLine("$$PWD/Register.h \\");
                    for (var x = 0; x < _classes.Count; x++)
                    {
                        if (x == _classes.Count - 1)
                        {
                            writer.WriteLine($"$$PWD/{_classes[x].CppName}.h");
                        }
                        else
                        {
                            writer.WriteLine($"$$PWD/{_classes[x].CppName}.h \\");
                        }
                    }
                }

                writer.WriteLine("SOURCES += \\");
                using (writer.BeginIndent())
                {
                    writer.WriteLine("$$PWD/Register.cpp \\");
                    for (var x = 0; x < _classes.Count; x++)
                    {
                        if (x == _classes.Count - 1)
                        {
                            writer.WriteLine($"$$PWD/{_classes[x].CppName}.cpp");
                        }
                        else
                        {
                            writer.WriteLine($"$$PWD/{_classes[x].CppName}.cpp \\");
                        }
                    }
                }
            }

            using (var writer = new CodeWriter(Path.Combine(directory, "Register.h")))
            {
                writer.WriteLine("#include <QCoreApplication>");
                writer.WriteLine("Q_DECL_EXPORT void initAotTypes();");
            }
            
            using (var writer = new CodeWriter(Path.Combine(directory, "Register.cpp")))
            {
                writer.WriteLine("#include \"Register.h\"");
                writer.WriteLine("#include <QCoreApplication>");
                writer.WriteLine("#include <QMutex>");
                writer.WriteLine("#include <QmlNet/types/NetTypeManager.h>");
                foreach (var cls in _classes)
                {
                    writer.WriteLine($"#include \"{cls.CppName}.h\"");
                }
                writer.WriteLine("static bool initAotTypesDone = false;");
                writer.WriteLine("Q_GLOBAL_STATIC(QMutex, initAotTypesMutex);");
                writer.WriteLine("");
                writer.WriteLine("Q_DECL_EXPORT void initAotTypes()");
                writer.WriteLine("{");
                using (writer.BeginIndent())
                {
                    writer.WriteLine("initAotTypesMutex->lock();");
                    writer.WriteLine("if(initAotTypesDone) {");
                    using (writer.BeginIndent())
                    {
                        writer.WriteLine("initAotTypesMutex->unlock();");
                        writer.WriteLine("return;");
                    }
                    writer.WriteLine("}");
                    foreach (var cls in _classes)
                    {
                        writer.WriteLine($"NetTypeManager::registerAotObject({cls.TypeId}, &{cls.CppName}::staticMetaObject, {cls.CppName}::_registerQml, {cls.CppName}::_registerQmlSingleton);");
                    }
                    writer.WriteLine("initAotTypesDone = true;");
                    writer.WriteLine("initAotTypesMutex->unlock();");
                }
                writer.WriteLine("}");
                writer.WriteLine("");
                writer.WriteLine("Q_COREAPP_STARTUP_FUNCTION(initAotTypes)");
            }

            foreach (var cls in _classes)
            {
                cls.WriteCpp(directory, _classes);
            }
        }

        public void WriteNetCode(string directory)
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
            
            if (_classes.All(x => x.Type != typeof(object)))
            {
                _classes.Add(new AotClass(typeof(object), Interlocked.Increment(ref _aotTypeIdCounter)));
            }
            
            using (var writer = new CodeWriter(Path.Combine(directory, $"{_options.Name}.cs")))
            {
                writer.WriteLine("using Qml.Net.Aot;");
                writer.WriteLine("// ReSharper disable once CheckNamespace");
                writer.WriteLine($"namespace {_options.NetNamespace}");
                writer.WriteLine("{");
                using (writer.BeginIndent())
                {
                    writer.WriteLine($"public class {_options.Name}");
                    writer.WriteLine("{");
                    using (writer.BeginIndent())
                    {
                        writer.WriteLine("private static bool _didRegister;");
                        writer.WriteLine("private static readonly object Lock = new object();");
                        writer.WriteLine("public static void Register()");
                        writer.WriteLine("{");
                        using (writer.BeginIndent())
                        {
                            writer.WriteLine("lock (Lock)");
                            writer.WriteLine("{");
                            using (writer.BeginIndent())
                            {
                                writer.WriteLine("if (_didRegister) return;");
                                foreach (var cls in _classes)
                                {
                                    // ReSharper disable once PossibleNullReferenceException
                                    writer.WriteLine(
                                        $"AotTypes.AddAotType({cls.TypeId}, typeof(global::{cls.Type.FullName.Replace("+", ".")}));");
                                }
                                writer.WriteLine("_didRegister = true;");
                            }
                            writer.WriteLine("}");
                        }
                        writer.WriteLine("}");
                    }
                    writer.WriteLine("}");
                }
                writer.WriteLine("}");
            }
        }
        
        public void Dispose()
        {
        }
    }
}