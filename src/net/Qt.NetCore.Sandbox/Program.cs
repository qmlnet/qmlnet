using System;

namespace Qt.NetCore.Sandbox
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = System.Environment.GetEnvironmentVariable("PATH");
            path += ";" + @"D:\Git\Github\pauldotknopf\net-core-qml\src\native\build-QtNetCoreQml-Visual_Studio_64bit-Debug\debug";
            System.Environment.SetEnvironmentVariable("PATH", path);
            Qt.NetCore.Class1.Test();
        }
    }
}