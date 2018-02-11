using System;
using System.IO;
using System.Security.Cryptography;

namespace MiniCover.Utils
{
    public static class FileUtils
    {
        public static string GetFileHash(string file)
        {
            using (var stream = File.OpenRead(file))
            {
                using (var hasher = new MD5CryptoServiceProvider())
                {
                    var hashBytes = hasher.ComputeHash(stream);
                    return BitConverter.ToString(hashBytes).Replace("-", string.Empty);
                }
            }
        }
    }
}