using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace MiniCover
{
    public static class HitService
    {
        private static readonly object lockObject = new object();
        private static Dictionary<string, Hits> files = new Dictionary<string, Hits>();


        public static void Init(string fileName)
        {
            lock (lockObject)
            {
                if (!files.ContainsKey(fileName))
                {
                    files[fileName] = new Hits();
                }
            }
        }

        public static void Hit(string fileName, int id)
        {
            lock (lockObject)
            {
                var hits = files[fileName];
                hits.Hited(id);
            }
        }


        static void Save()
        {
            var binaryFormatter = new BinaryFormatter();

            foreach (var file in files)
            {
                using (var stream = File.Open(file.Key, FileMode.Append, FileAccess.Write, FileShare.None))
                {
                    foreach (var kv in file.Value)
                    {
                        binaryFormatter.Serialize(stream, kv);
                    }

                    stream.Flush();
                }
            }

        }

        static HitService()
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => Save();
        }
    }
}