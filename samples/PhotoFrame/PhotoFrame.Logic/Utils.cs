using System;
using System.IO;
using System.Reflection;

namespace PhotoFrame.Logic
{
    public static class Utils
    {
        private static readonly string QML_RELATIVE_PATH = $"UI{Path.DirectorySeparatorChar}QML";

        public static string GetQmlRelativePath(string absolutePath)
        {
            string qmlAbsolutePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), QML_RELATIVE_PATH);
            return MakeRelativePath(qmlAbsolutePath, absolutePath);
        }

        public static String MakeRelativePath(String fromPath, String toPath)
        {
            if (String.IsNullOrEmpty(fromPath)) throw new ArgumentNullException("fromPath");
            if (String.IsNullOrEmpty(toPath)) throw new ArgumentNullException("toPath");

            //ensure that directories end with the directory separator char
            if(Directory.Exists(fromPath) && !fromPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                fromPath += Path.DirectorySeparatorChar;
            }
            if (Directory.Exists(toPath) && !toPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                toPath += Path.DirectorySeparatorChar;
            }

            Uri fromUri = new Uri(fromPath);
            Uri toUri = new Uri(toPath);

            if (fromUri.Scheme != toUri.Scheme) { return toPath; } // path can't be made relative.

            Uri relativeUri = fromUri.MakeRelativeUri(toUri);
            String relativePath = Uri.UnescapeDataString(relativeUri.ToString());

            if (toUri.Scheme.Equals("file", StringComparison.InvariantCultureIgnoreCase))
            {
                relativePath = relativePath.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }

            return relativePath;
        }
    }
}
