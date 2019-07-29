using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace Qml.Net.Aot
{
    public class AotClass
    {
        private readonly Type _type;
        private readonly List<AotMethod> _methods = new List<AotMethod>();
        
        public AotClass(Type type, int typeId)
        {
            _type = type;
            TypeId = typeId;
        }

        public int TypeId { get; }
        
        public Type Type => _type;

        public string CppName => $"Net{_type.Name}";
        
        public ReadOnlyCollection<AotMethod> Methods => new ReadOnlyCollection<AotMethod>(_methods);

        internal void AddMethod(MethodInfo methodInfo)
        {
            _methods.Add(new AotMethod(this, methodInfo));
        }

        internal void WriteCpp(string directory, List<AotClass> allClasses)
        {
            var headerFile = Path.Combine(directory, $"{CppName}.h");
            var cppFile = Path.Combine(directory, $"{CppName}.cpp");
                
            using (var stream = File.OpenWrite(headerFile))
            using (var writer = new StreamWriter(stream))
            {
                AotClass aotBaseClass = null;
                {
                    var baseType = Type.BaseType;
                    while (baseType != null)
                    {
                        aotBaseClass = allClasses.Single(x => x.Type == baseType);
                        if (aotBaseClass != null)
                        {
                            break;
                        }
                        baseType = baseType.BaseType;
                    }
                }
                
                if (aotBaseClass != null)
                {
                    writer.WriteLine($"#include \"{aotBaseClass.CppName}.h\"");
                    writer.WriteLine($"class {CppName} : public {aotBaseClass.CppName}");
                }
                else
                {
                    writer.WriteLine("#include <QObject>");
                    writer.WriteLine($"class {CppName}: public QObject");
                }
                writer.WriteLine("{");
                writer.WriteLine("};");
            }
                
            using (var stream = File.OpenWrite(cppFile))
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine($"#include \"{CppName}.h\"");
            }
        }
    }
}