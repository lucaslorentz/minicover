using System.Collections.Generic;
using System.IO;

namespace MiniCover
{
    public static class HitService
    {
        private static readonly object lockObject = new object();
        private static Dictionary<string, StreamWriter> writers = new Dictionary<string, StreamWriter>();

        public static void Init(string fileName)
        {
            lock (lockObject)
            {
                if (!writers.ContainsKey(fileName))
                {
                    writers[fileName] = new StreamWriter(File.Open(fileName, FileMode.Append));
                }
            }
        }

        public static void Hit(string fileName, int id)
        {
            lock (lockObject)
            {
                var streamWriter = writers[fileName];
                streamWriter.WriteLine(id);
                streamWriter.Flush();
            }
        }
    }
}
