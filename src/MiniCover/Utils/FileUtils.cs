using System;
using System.IO;
using System.Security.Cryptography;

namespace MiniCover.Utils
{
    public static class FileUtils
    {
        public static string GetFileHash(FileInfo file)
        {
            using (var stream = file.OpenRead())
            {
                using (var hasher = new MD5CryptoServiceProvider())
                {
                    var hashBytes = hasher.ComputeHash(stream);
                    return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                }
            }
        }

        public static DirectoryInfo AddEndingDirectorySeparator(this DirectoryInfo info)
        {
            if (info.FullName.EndsWith(Path.DirectorySeparatorChar)
                || info.FullName.EndsWith(Path.AltDirectorySeparatorChar))
                return info;

            return new DirectoryInfo($"{info.FullName}{Path.DirectorySeparatorChar}");
        }

        public static FileInfo GetPdbFile(FileInfo assemblyFile)
        {
            return new FileInfo(Path.ChangeExtension(assemblyFile.FullName, "pdb"));
        }

        public static FileInfo GetBackupFile(FileInfo file)
        {
            return new FileInfo(Path.ChangeExtension(file.FullName, $"uninstrumented{file.Extension}"));
        }

        public static bool IsBackupFile(FileInfo file)
        {
            return file.Name.Contains(".uninstrumented");
        }
    }
}