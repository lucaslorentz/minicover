using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;

namespace MiniCover
{
    public static class HitService
    {
        private static ConcurrentDictionary<string, ConcurrentDictionary<int, Hit>> files = new ConcurrentDictionary<string, ConcurrentDictionary<int, Hit>>();

        //private static readonly ConcurrentDictionary<string, Hits> files = new ConcurrentDictionary<string, Hits>();
        private static int counter;
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
                using(var fileStream = File.Open(file.Key, FileMode.Append, FileAccess.Write, FileShare.None)) 
                using (var streamWriter = new StreamWriter(fileStream))
                {
                    if(fileStream.Position != 0) streamWriter.Write(",");
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
            private static AsyncLocal<TestMethodInfo> _testMethodCache = new AsyncLocal<TestMethodInfo>();

            private ConcurrentDictionary<int, Hit> _hitInstructions;
            private TestMethodInfo _testMethod;
            private bool _clearTestMethodCache;

            public MethodContext(ConcurrentDictionary<int, Hit> hitInstructions)
            {
                _hitInstructions = hitInstructions;

                if (_testMethodCache.Value == null)
                {
                    _testMethodCache.Value = TestMethodInfo.GetCurrentTestMethodInfo();
                    _clearTestMethodCache = true;
                }

                _testMethod = _testMethodCache.Value;
            }

            public void HitInstruction(int id)
            {
                var hitInstruction = _hitInstructions.GetOrAdd(id, i => new Hit(i));
                hitInstruction.HitedBy(_testMethod);
            }

            public void Exit()
            {
                if (_clearTestMethodCache)
                    _testMethodCache.Value = null;
            }
        }
    }
}