using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;

namespace MiniCover
{
    public static class HitService
    {
        private static object lockObject = new object();
        private static Dictionary<string, StreamWriter> writers = new Dictionary<string, StreamWriter>();

        public static void Init(string fileName)
        {
            lock (lockObject)
            {
                if (!writers.ContainsKey(fileName))
                {
                    writers[fileName] = new StreamWriter(File.Open(fileName, FileMode.OpenOrCreate));
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
