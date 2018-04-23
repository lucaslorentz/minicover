using Microsoft.Extensions.FileSystemGlobbing.Abstractions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public static string[] GetFiles(List<string> includeOptionValues, List<string> excludeOptionValues, string defaultInclude, string parentDir)
        {
            var matcher = new Microsoft.Extensions.FileSystemGlobbing.Matcher();

            if (includeOptionValues != null && includeOptionValues.Count > 0)
            {
                foreach (var include in includeOptionValues)
                {
                    matcher.AddInclude(include);
                }
            }
            else if (!string.IsNullOrEmpty(defaultInclude))
            {
                matcher.AddInclude(defaultInclude);
            }

            if (excludeOptionValues != null)
            {
                foreach (var exclude in excludeOptionValues)
                {
                    matcher.AddExclude(exclude);
                }
            }

            DirectoryInfo directoryInfo = null;
            if (!string.IsNullOrEmpty(parentDir))
            {
                directoryInfo = new DirectoryInfo(parentDir);
            }
            else
            {
                directoryInfo = new DirectoryInfo(Directory.GetCurrentDirectory());
            }
            var fileMatchResult = matcher.Execute(new DirectoryInfoWrapper(directoryInfo));
            return fileMatchResult.Files
                .Select(f => Path.GetFullPath(Path.Combine(directoryInfo.ToString(), f.Path)))
                .ToArray();
        }
    }
}