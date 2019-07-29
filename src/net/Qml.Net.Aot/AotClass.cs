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
            AotClass aotBaseClass = null;
            
            using (var writer = new CodeWriter(headerFile))
            {
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

                writer.WriteLine($"#ifndef {CppName.ToUpper()}_H");
                writer.WriteLine($"#define {CppName.ToUpper()}_H");
                
                if (aotBaseClass != null)
                {
                    writer.WriteLine($"#include \"{aotBaseClass.CppName}.h\"");
                    writer.WriteLine("class QQmlEngine;");
                    writer.WriteLine("class QJSEngine;");
                    writer.WriteLine($"class {CppName} : public {aotBaseClass.CppName}");
                }
                else
                {
                    writer.WriteLine("#include <QObject>");
                    writer.WriteLine("#include <QmlNet/types/NetReference.h>");
                    writer.WriteLine("class QQmlEngine;");
                    writer.WriteLine("class QJSEngine;");
                    writer.WriteLine($"class {CppName}: public QObject");
                }
                writer.WriteLine("{");
                using (writer.BeginIndent())
                {
                    writer.WriteLine("Q_OBJECT");
                }
                writer.WriteLine("public:");
                using (writer.BeginIndent())
                {
                    writer.WriteLine($"{CppName}();");
                    writer.WriteLine($"{CppName}(bool);");
                    writer.WriteLine("static int _registerQml(const char* uri, int versionMajor, int versionMinor, const char* qmlName);");
                    writer.WriteLine("static int _registerQmlSingleton(const char* uri, int versionMajor, int versionMinor, const char* typeName);");
                    writer.WriteLine("static QObject* _build(QQmlEngine* engine, QJSEngine* scriptEngine);");
                    foreach (var method in _methods)
                    {
                        writer.WriteLine($"Q_INVOKABLE void {method.MethodName}();");
                    }
                }

                if (aotBaseClass == null)
                {
                    writer.WriteLine("protected:");
                    using (writer.BeginIndent())
                    {
                        writer.WriteLine("QSharedPointer<NetReference> _netReference;");
                    }
                }
                writer.WriteLine("};");
                
                writer.WriteLine($"#endif // {CppName.ToUpper()}_H");
            }
            
            using (var writer = new CodeWriter(cppFile))
            {
                writer.WriteLine($"#include \"{CppName}.h\"");
                writer.WriteLine("#include <QQmlApplicationEngine>");
                writer.WriteLine("#include <QmlNet/types/Callbacks.h>");
                if (aotBaseClass != null)
                {
                    writer.WriteLine($"{CppName}::{CppName}() : {aotBaseClass.CppName}(false)");
                }
                else
                {
                    writer.WriteLine($"{CppName}::{CppName}() : QObject()");
                }
                writer.WriteLine("{");
                using (writer.BeginIndent())
                {
                    writer.WriteLine($"_netReference = QmlNet::instantiateType(nullptr, {TypeId});");
                }
                writer.WriteLine("}");
                if (aotBaseClass != null)
                {
                    writer.WriteLine($"{CppName}::{CppName}(bool) : {aotBaseClass.CppName}(false)");
                }
                else
                {
                    writer.WriteLine($"{CppName}::{CppName}(bool) : QObject()");
                }
                writer.WriteLine("{");
                writer.WriteLine("}");
                writer.WriteLine($"int {CppName}::_registerQml(const char* uri, int versionMajor, int versionMinor, const char* qmlName)");
                writer.WriteLine("{");
                using (writer.BeginIndent())
                {
                    writer.WriteLine($"return qmlRegisterType<{CppName}>(uri, versionMajor, versionMinor, qmlName);");
                }
                writer.WriteLine("}");
                writer.WriteLine($"int {CppName}::_registerQmlSingleton(const char* uri, int versionMajor, int versionMinor, const char* typeName)");
                writer.WriteLine("{");
                using (writer.BeginIndent())
                {
                    writer.WriteLine($"return qmlRegisterSingletonType<{CppName}>(uri, versionMajor, versionMinor, typeName, {CppName}::_build);");
                }
                writer.WriteLine("}");
                writer.WriteLine($"QObject* {CppName}::_build(QQmlEngine *engine, QJSEngine *scriptEngine)");
                writer.WriteLine("{");
                using (writer.BeginIndent())
                {
                    writer.WriteLine("Q_UNUSED(engine)");
                    writer.WriteLine("Q_UNUSED(scriptEngine)");
                    writer.WriteLine($"return new {CppName}();");
                }
                writer.WriteLine("}");
                foreach (var method in _methods)
                {
                    writer.WriteLine($"void {CppName}::{method.MethodName}()");
                    writer.WriteLine("{");
                    writer.WriteLine("}");
                }
            }
        }
    }
}