using System.IO;
using System.Runtime.InteropServices;

namespace MiniCover.UnitTests.TestHelpers
{
    public static class StringExtensions
    {
        public static string ToOSLineEnding(this string text)
        {
            text = text.Replace("\r\n", "\n");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                text = text.Replace("\n", "\r\n");

            return text;
        }

        public static string ToOSPath(this string text)
        {
            if (text.StartsWith("/") && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                text = $"c:{text}";
            else if (text.StartsWith("c:") && !RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                text = text.Substring(2);

            return text.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        }
    }
}
