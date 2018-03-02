using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

namespace MiniCover
{
    public static class HitServiceWithTests
    {
        private static readonly object lockObject = new object();
        private static Dictionary<string, Dictionary<int, HitTrace>> files = new Dictionary<string, Dictionary<int, HitTrace>>();
        private static Dictionary<string, bool> assemblyPdb = new Dictionary<string, bool>();
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
                StackTrace stackTrace = new StackTrace();

                var testMethod = GetTestMethod(stackTrace);
                
                var hits = files[fileName];
                if (!hits.ContainsKey(id))
                    hits.Add(id, new HitTrace(id));
                hits[id].Hited(testMethod);
            }
        }

        private static MethodBase GetTestMethod(StackTrace stack)
        {
            var frames = stack.GetFrames();
            for (int i = frames.Length - 1; i >= 0; i--)
            {
                var currentMethod = frames[i].GetMethod();
                if (currentMethod.HasPdb())
                    return currentMethod;
            }

            return null;
        }

        private static bool HasPdb(this MethodBase methodBase)
        {
            var location = methodBase.DeclaringType.Assembly.Location;

            if (!assemblyPdb.TryGetValue(location, out var hasPdb))
            {
                assemblyPdb[location] = hasPdb = File.Exists(Path.ChangeExtension(location, ".pdb"));
            }
            return hasPdb;
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