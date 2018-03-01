using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;

namespace MiniCover
{
    public static class HitServiceWithTests
    {
        private static readonly object lockObject = new object();
        private static Dictionary<string, Dictionary<int, HitTrace>> files = new Dictionary<string, Dictionary<int, HitTrace>>();

        public static void Init(string fileName)
        {
            lock (lockObject)
            {

                if (!files.ContainsKey(fileName))
                {
                    files[fileName] = new Dictionary<int, HitTrace>();
                }
            }
        }

        public static void Hit(string fileName, int id)
        {
            lock (lockObject)
            {
                StackTrace stackTrace = new System.Diagnostics.StackTrace();

                var currentIndex = 0;

                var frames = stackTrace.GetFrames();
                foreach (var currentFrame in frames)
                {
                    if (currentFrame.GetMethod().Name == "InvokeMethod")
                        break;
                    currentIndex++;
                }
                var frame = frames[currentIndex - 1];
                var hits = files[fileName];
                if (!hits.ContainsKey(id))
                    hits.Add(id, new HitTrace(id));
                hits[id].Hited(frame.GetMethod());
            }
        }

        static void Save()
        {
            foreach (var fileName in files)
            {
                using (var streamWriter = new StreamWriter(File.Open(fileName.Key, FileMode.Append)))
                {
                    foreach (var hitTrace in fileName.Value)
                    {
                        hitTrace.Value.WriteInformation(streamWriter);
                    }

                    streamWriter.Flush();
                }
            }

        }

        static HitServiceWithTests()
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => Save();
        }

        private class HitTrace
        {
            private readonly int integrationId;

            private readonly IList<MethodBase> hits = new List<MethodBase>();

            public HitTrace(int integrationId)
            {
                this.integrationId = integrationId;
            }

            internal void Hited(MethodBase method)
            {
                hits.Add(method);
            }

            internal void WriteInformation(StreamWriter writer)
            {
                foreach (var hit in hits)
                {
                    writer.WriteLine(
                        $"{integrationId}:{hit.DeclaringType.Assembly.FullName}:{hit.DeclaringType.Name}:{hit.Name}:{hit.DeclaringType.Assembly.Location}");
                }
            }
        }
    }
}