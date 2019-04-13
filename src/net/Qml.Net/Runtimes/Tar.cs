using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Schema;

namespace Qml.Net.Runtimes
{
    internal class Tar
    {
        public enum EntryType : byte
        {
            // ReSharper disable UnusedMember.Global
            File = 0,
            OldFile = (byte)'0',
            HardLink = (byte)'1',
            SymLink = (byte)'2',
            CharDevice = (byte)'3',
            BlockDevice = (byte)'4',
            Directory = (byte)'5',
            Fifo = (byte)'6',
            LongLink = (byte)'K',
            LongName = (byte)'L',
            SparseFile = (byte)'S',
            VolumeHeader = (byte)'V',
            GlobalExtendedHeader = (byte)'g'
            // ReSharper restore UnusedMember.Global
        }

        public static Header ReadHeader(Stream stream)
        {
            var buffer = new byte[512];
            var bytesRead = stream.Read(buffer, 0, buffer.Length);

            if (bytesRead != 512)
            {
                throw new Exception("Couldn't ready block");
            }

            if (buffer.All(singleByte => singleByte == 0))
            {
                // end of archive
                return null;
            }

            var header = new Header();
            header.EntryType = (EntryType)buffer[156];

            switch (header.EntryType)
            {
                case EntryType.File:
                case EntryType.OldFile:
                case EntryType.Directory:
                    break;
                case EntryType.SymLink:
                    header.LinkName = Encoding.ASCII.GetString(buffer, 157, 100).Trim('\0', ' ');
                    break;
                default:
                    throw new Exception($"Unsupported type: {header.EntryType}");
            }

            header.Name = Encoding.ASCII.GetString(buffer, 0, 100).Trim('\0', ' ');
            header.Size = Convert.ToUInt64(Encoding.ASCII.GetString(buffer, 124, 12).Trim('\0', ' '), 8);
            header.Mode = Convert.ToInt32(Encoding.ASCII.GetString(buffer, 100, 8).Trim('\0', ' '), 8);

            return header;
        }

        public class Header
        {
            public EntryType EntryType { get; set; }

            public string Name { get; set; }

            public string LinkName { get; set; }

            public ulong Size { get; set; }

            public int Mode { get; set; }
        }

        public static void ExtractTarFromGzipStream(Stream stream, string destinationDirectory)
        {
            using (var gstream = new GZipStream(stream, CompressionMode.Decompress))
            {
                var isUnix = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ||
                             RuntimeInformation.IsOSPlatform(OSPlatform.OSX);

                var fileBuffer = new byte[1024 * 4];

                while (true)
                {
                    var header = ReadHeader(gstream);

                    if (header == null)
                    {
                        break;
                    }

                    var output = Path.Combine(destinationDirectory, header.Name);

                    if (header.EntryType == EntryType.SymLink)
                    {
                        if (!isUnix)
                        {
                            throw new Exception("Cannot extract symlink on current platform.");
                        }

                        var parentDirectory = Path.GetDirectoryName(output);
                        if (!Directory.Exists(parentDirectory))
                        {
                            Directory.CreateDirectory(parentDirectory);
                        }

                        Symlink.Create(header.LinkName, output);
                    }

                    if (header.Size > 0)
                    {
                        var parentDirectory = Path.GetDirectoryName(output);
                        if (!Directory.Exists(parentDirectory))
                        {
                            Directory.CreateDirectory(parentDirectory);
                        }

                        using (var fs = File.Open(output, FileMode.Create, FileAccess.Write))
                        {
                            var byteToRead = header.Size;
                            while (byteToRead > 0)
                            {
                                var bytesRead = gstream.Read(
                                    fileBuffer,
                                    0,
                                    (int)Math.Min(byteToRead, (ulong)fileBuffer.Length));
                                if (bytesRead == 0)
                                {
                                    throw new Exception("Couldn't read bytes.");
                                }

                                fs.Write(fileBuffer, 0, bytesRead);

                                byteToRead -= (ulong)bytesRead;
                            }
                        }

                        if (isUnix)
                        {
                            // This OS supports file modes.
                            // Let's set them.
                            Chmod.Set(output, header.Mode);
                        }

                        var trailing = 512 - (int)(header.Size % 512);
                        if (trailing == 512)
                        {
                            trailing = 0;
                        }
                        if (trailing > 0)
                        {
                            if (gstream.Read(fileBuffer, 0, trailing) != trailing)
                            {
                                throw new Exception("Couldn't read bytes");
                            }
                        }
                    }
                }
            }
        }
    }
}