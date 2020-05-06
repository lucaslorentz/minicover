using System.IO;

namespace MiniCover.Utils
{
    public static class ResourceUtils
    {
        public static string GetContent(string resourceName)
        {
            var assembly = typeof(ResourceUtils).Assembly;

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(stream))
            {
                return reader.ReadToEnd();
            }
        }
    }
}
