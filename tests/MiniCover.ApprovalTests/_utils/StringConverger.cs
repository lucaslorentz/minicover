using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;

namespace MiniCover.ApprovalTests
{
    internal static class StringConverger
    {
        internal static string ApplyCleanup(string text, params string[] pathToReplace)
        {

            foreach (var toRemove in pathToReplace)
            {
                var path = toRemove;
                if (!toRemove.EndsWith(Path.DirectorySeparatorChar))
                {
                    path = toRemove + Path.DirectorySeparatorChar;
                }
                var pattern = $"\"{Regex.Escape(Regex.Escape(path))}(.*?)\"";
                var matches = Regex.Matches(text, pattern);
                text = Regex.Replace(text, pattern, m => $"\"{CleanPath(m.Groups[1].Value)}\"");
                //var pathNormalized = CleanPath(path);
                //var pattern = $"\\\"{pathNormalized.Replace("\\", "\\\\")}(.*?)\\\"";
                //text = Regex.Replace(text, pattern, m => $"\"{m.Groups[1].Value}\"");
            }
            if(RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                text = Regex.Replace(text, $"\"..{Regex.Escape(Regex.Escape($"{Path.DirectorySeparatorChar}"))}(.*?)\"", m => $"\"../{CleanPath(m.Groups[1].Value)}\"");
            return text.Replace("\r\n", "\n");
        }

        private static string CleanPath(string path)
        {
            return path.Replace($"{Path.DirectorySeparatorChar}{Path.DirectorySeparatorChar}", "/");
        }
    }
}
