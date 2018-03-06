using MiniCover.Utils;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace MiniCover
{
    public static class HitService
    {
        private static readonly ConcurrentDictionary<string, ConcurrentDictionary<int, Hit>> files = new ConcurrentDictionary<string, ConcurrentDictionary<int, Hit>>();

        public static void Init(string fileName)
        {
            files.GetOrAdd(fileName, (f) => new ConcurrentDictionary<int, Hit>());
        }

        public static MethodContext EnterMethod(string fileName)
        {
            var file = files.GetOrAdd(fileName, f => new ConcurrentDictionary<int, Hit>());

            return new MethodContext(file);
        }

        static void Save()
        {
            foreach (var file in files)
            {
                using (var fileStream = File.Open(file.Key, FileMode.Append, FileAccess.Write, FileShare.None))
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    if (fileStream.Position != 0) streamWriter.Write(",");
                    var json = Newtonsoft.Json.JsonConvert.SerializeObject(file.Value.Select(a => a.Value));
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

        public class MethodContext
        {
            private static readonly AsyncLocal<MethodBase> TestMethodCache = new AsyncLocal<MethodBase>();

            private readonly ConcurrentDictionary<int, Hit> hitInstructions;
            private readonly MethodBase testMethod;
            private readonly bool clearTestMethodCache;

            public MethodContext(ConcurrentDictionary<int, Hit> hitInstructions)
            {
                this.hitInstructions = hitInstructions;

                if (TestMethodCache.Value == null)
                {
                    TestMethodCache.Value = TestMethodUtils.GetTestMethod();
                    clearTestMethodCache = true;
                }

                testMethod = TestMethodCache.Value;
            }

            public void HitInstruction(int id)
            {
                var hitInstruction = hitInstructions.GetOrAdd(id, i => new Hit(i));
                hitInstruction.HitedBy(testMethod);
            }

            public void Exit()
            {
                if (clearTestMethodCache)
                    TestMethodCache.Value = null;
            }
        }
    }
}