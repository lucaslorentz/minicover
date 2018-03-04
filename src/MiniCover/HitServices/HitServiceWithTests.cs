using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using MiniCover.HitServices;
using MiniCover.Utils;

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
                if (TestMethodCall.Current == null)
                {
                    var testMethod = OptimizedStackTrace.GetTestMethod(HasPdb);
                    TestMethodCall.Current = TestMethodCall.Create(testMethod);
                }
                
                var hits = files[fileName];
                if (!hits.ContainsKey(id))
                    hits.Add(id, new HitTrace(id));
                hits[id].Hited(TestMethodCall.Current.MethodBase);
            }
        }

        private static bool HasPdb(MethodBase methodBase)
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

            private readonly IDictionary<MethodBase, int> hits = new Dictionary<MethodBase, int>();

            public HitTrace(int integrationId)
            {
                this.integrationId = integrationId;
            }

            internal void Hited(MethodBase method)
            {
                if (!hits.ContainsKey(method))
                {
                    hits.Add(method, 0);
                }
                hits[method]++;
            }

            internal void WriteInformation(StreamWriter writer)
            {
                foreach (var hit in hits)
                {
                    writer.WriteLine(
                        $"{integrationId}:{hit.Key.DeclaringType.Assembly.FullName}:{hit.Key.DeclaringType.Name}:{hit.Key.Name}:{hit.Key.DeclaringType.Assembly.Location}:{hit.Value}");
                }
            }
        }
    }
}