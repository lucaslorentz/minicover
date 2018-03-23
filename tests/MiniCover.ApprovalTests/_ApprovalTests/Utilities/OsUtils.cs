using System.IO;

namespace ApprovalUtilities.Utilities
{
    public enum ApprovalsPlatform
    {
        Windows,
        Linux,
        Mac
    }

    public class OsUtils
    {
        public static string GetFullOsNameFromWmi()
        {
            return System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        }
        public static bool IsWindowsOs()
        {
            return Path.DirectorySeparatorChar == '\\';
        }

        public static bool IsUnixOs()
        {
            return !IsWindowsOs();
        }
    }
}