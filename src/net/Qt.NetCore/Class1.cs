using System;
using System.Runtime.InteropServices;

namespace Qt.NetCore
{
    public class Class1
    {
        [DllImport("QtNetCoreQml")]
        public static extern void Test();
    }
}
