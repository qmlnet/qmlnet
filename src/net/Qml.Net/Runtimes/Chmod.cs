using System;
using System.Runtime.InteropServices;

namespace Qml.Net.Runtimes
{
    internal static class Chmod
    {
        [DllImport("libc", SetLastError = true)]
        private static extern int chmod(string pathname, int mode);

        public static void Set(string pathName, int mode)
        {
            if (chmod(pathName, mode) != 0)
            {
                throw new Exception($"Unable to set mode: {pathName}");
            }
        }
    }
}