using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qt.NetCore.Sandbox
{
    class Program
    {
        static int Main(string[] args)
        {
            var path = System.Environment.GetEnvironmentVariable("PATH");
            path += ";" + @"D:\Git\Github\pauldotknopf\net-core-qml\src\native\build-QtNetCoreQml-Visual_Studio_64bit-Debug\debug";
            System.Environment.SetEnvironmentVariable("PATH", path);

            using (var r = new StringVector(0))
            {
                using (var app = new QGuiApplication(r))
                {
                    return app.exec();
                }
            }
        }
    }
}
