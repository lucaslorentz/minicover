using MiniCover.HitServices;
using System;
using System.Collections.Concurrent;
using System.IO;

namespace MiniCover
{
    public static class HitService
    {
        private static readonly ConcurrentDictionary<string, Hits> files = new ConcurrentDictionary<string, Hits>();

        public static void Init(string fileName)
        {
            files.GetOrAdd(fileName, f => new Hits());
        }

        public static void Hit(string fileName, int id)
        {
            var hits = files[fileName];
            hits.Hited(id);
        }


        static void Save()
        {
            foreach (var file in files)
            {
                using(var fileStream = File.Open(file.Key, FileMode.Append, FileAccess.Write, FileShare.None)) 
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    if(fileStream.Position != 0) streamWriter.Write(",");
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(file.Value);
                    json = json.Substring(1, json.Length - 2);
                    streamWriter.Write(json);
                    streamWriter.Flush();
                }
            }
        }

        static HitService()
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => Save();
        }
    }
}