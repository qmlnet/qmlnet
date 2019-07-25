using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using Qml.Net.Aot;
using Qml.Net.Tests.Qml;

namespace Qml.Net.Tests.Aot
{
    public abstract class AotTestsBase : BaseQmlTests<>
    {
        private string _workingDirectory;

        protected AotTestsBase()
        {
            //_workingDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, Guid.NewGuid().ToString().Replace("-", ""));
            _workingDirectory = "/home/pknopf/git/qmlnet/src/net/interop";
            if (Directory.Exists(_workingDirectory))
            {
                Directory.Delete(_workingDirectory, true);
            }
            Directory.CreateDirectory(_workingDirectory);
        }

        private string GetQmlNetPriLocation()
        {
            var recursive = 0;
            var directory = _workingDirectory;
            
            while (!Directory.Exists(Path.Combine(directory, "native")))
            {
                directory = Directory.GetParent(directory).FullName;
                recursive++;
            }

            var path = new StringBuilder();

            for (var x = 0; x < recursive; x++)
            {
                path.Append("../");
            }

            path.Append($"native{Path.DirectorySeparatorChar}QmlNet{Path.DirectorySeparatorChar}QmlNet.pri");

            return path.ToString();
        }

        protected void GenerateNativeBuildFiles(Action<AotSession> callback)
        {
            using (var aotSession = new AotSession())
            {
                callback(aotSession);
                aotSession.WriteNativeCode(_workingDirectory);
            }
            
            // Write the .pro file which will be the lib that our native code will go into.
            using (var stream = File.OpenWrite(InteropProFileLocation))
            using (var writer = new StreamWriter(stream))
            {
                writer.WriteLine("CONFIG += c++11");
                writer.WriteLine("CONFIG += plugin");
                writer.WriteLine();
                writer.WriteLine("TARGET = QmlNet");
                writer.WriteLine("TEMPLATE = lib");
                writer.WriteLine();
                writer.WriteLine("DEFINES += QT_DEPRECATED_WARNINGS");
                writer.WriteLine();
                writer.WriteLine($"include({GetQmlNetPriLocation()})");
                writer.WriteLine("include(interop-native.pri)");
            }
        }

        protected void RunNativeBuild()
        {
            CommandHelper.Run("qmake .", _workingDirectory);
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                CommandHelper.Run("jom", _workingDirectory);
            }
            else
            {
                CommandHelper.Run("make", _workingDirectory);
            }
        }
        
        protected string InteropProFileLocation => Path.Combine(_workingDirectory, "interop.pro");

        protected string WorkingDirectory => _workingDirectory;

        public virtual void MapSession(AotSession session)
        {
            
        }
        
        public void Dispose()
        {
            //Directory.Delete(_workingDirectory, true);
        }
    }
}