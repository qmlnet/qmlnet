using System;
using System.Runtime.InteropServices;

namespace Qml.Net.Runtimes
{
    internal static class Symlink
    {
        [DllImport("libc")]
        private static extern int symlink(string path1, string path2);

        public static void Create(string path1, string path2)
        {
            if (symlink(path1, path2) != 0)
            {
                throw new Exception($"Couldn't create symlink from {path1} to {path2}");
            }
        }
    }
}