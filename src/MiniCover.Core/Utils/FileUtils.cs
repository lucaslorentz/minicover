using System;
using System.IO;
using System.IO.Abstractions;
using System.Security.Cryptography;

namespace MiniCover.Core.Utils
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

        public static IFileInfo GetPdbFile(IFileInfo assemblyFile)
        {
            return assemblyFile.FileSystem.FileInfo.New(Path.ChangeExtension(assemblyFile.FullName, "pdb"));
        }

        public static IFileInfo GetBackupFile(IFileInfo file)
        {
            return file.FileSystem.FileInfo.New(Path.ChangeExtension(file.FullName, $"uninstrumented{file.Extension}"));
        }

        public static bool IsBackupFile(IFileInfo file)
        {
            return file.Name.Contains(".uninstrumented");
        }
    }
}