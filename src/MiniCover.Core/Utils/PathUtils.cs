using System;
using System.IO;

namespace MiniCover.Core.Utils
{
    public static class PathUtils
    {
        public static string GetRelativePath(string folder, string file)
        {
            if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString())
                && !folder.EndsWith(Path.AltDirectorySeparatorChar.ToString()))
                folder += Path.DirectorySeparatorChar;

            var baseUri = new Uri(folder);
            var fullUri = new Uri(file);

            return Uri.UnescapeDataString(
                baseUri.MakeRelativeUri(fullUri)
                .ToString()
                .Replace('/', Path.DirectorySeparatorChar));
        }
    }
}