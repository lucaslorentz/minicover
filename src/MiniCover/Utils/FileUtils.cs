using System;
using System.IO;
using System.IO.Abstractions;
using System.Security.Cryptography;

namespace MiniCover.Utils
{
    public static class FileUtils
    {
        public static string GetFileHash(IFileInfo file)
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

        public static IDirectoryInfo AddEndingDirectorySeparator(this IDirectoryInfo info)
        {
            if (info.FullName.EndsWith(Path.DirectorySeparatorChar)
                || info.FullName.EndsWith(Path.AltDirectorySeparatorChar))
                return info;

            return info.FileSystem.DirectoryInfo.FromDirectoryName($"{info.FullName}{Path.DirectorySeparatorChar}");
        }

        public static IFileInfo GetPdbFile(IFileInfo assemblyFile)
        {
            return assemblyFile.FileSystem.FileInfo.FromFileName(Path.ChangeExtension(assemblyFile.FullName, "pdb"));
        }

        public static IFileInfo GetBackupFile(IFileInfo file)
        {
            return file.FileSystem.FileInfo.FromFileName(Path.ChangeExtension(file.FullName, $"uninstrumented{file.Extension}"));
        }

        public static bool IsBackupFile(IFileInfo file)
        {
            return file.Name.Contains(".uninstrumented");
        }
    }
}