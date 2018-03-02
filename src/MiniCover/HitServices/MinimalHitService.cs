using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MiniCover.HitServices
{
    public static class MinimalHitService
    {
        private static readonly object lockObject = new object();
        private static readonly Dictionary<string, HitsTracer> files = new Dictionary<string, HitsTracer>();

        public static void Init(string fileName)
        {
            lock (lockObject)
            {

                if (!files.ContainsKey(fileName))
                {
                    var fileInfo = new FileInfo(fileName);
                    var tracer = files[fileName] = new HitsTracer();

                    using (var streamReader = new StreamReader(fileInfo.Open(FileMode.OpenOrCreate, FileAccess.Read)))
                    {
                        var lines = streamReader.ReadLines();
                        tracer.Parse(lines.ToArray());
                    }
                }
            }
        }

        private static IEnumerable<string> ReadLines(this StreamReader reader)
        {

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                yield return line;
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
            foreach (var fileName in files)
            {
                using (var streamWriter = new StreamWriter(File.Open(fileName.Key, FileMode.Create)))
                {
                    fileName.Value.Print(streamWriter);

                    streamWriter.Flush();
                }
            }

        }

        static MinimalHitService()
        {
            AppDomain.CurrentDomain.ProcessExit += (sender, args) => Save();
        }



        private class HitsTracer
        {
            private readonly Dictionary<int, HitTrace> traces;

            internal HitsTracer()
            {
                this.traces = new Dictionary<int, HitTrace>();
            }

            public void Hited(int id)
            {
                if (!this.traces.ContainsKey(id))
                {
                    this.traces.Add(id, new HitTrace(id));
                }

                this.traces[id].Hited();
            }


            public void Print(StreamWriter writer)
            {
                foreach (var trace in traces)
                {
                    trace.Value.WriteInformation(writer.WriteLine);
                }
            }

            public void Parse(string[] lines)
            {
                foreach (var line in lines)
                {
                    var trace = HitTrace.Parse(line);
                    this.traces.Add(trace.IntegrationId, trace);
                }
            }
        }
        private class HitTrace
        {
            internal int IntegrationId { get; }

            private int hits;

            public HitTrace(int integrationId)
                : this(integrationId, 0)
            {
            }

            private HitTrace(int integrationId, int currentCounter)
            {
                this.IntegrationId = integrationId;
                this.hits = currentCounter;
            }

            internal void Hited()
            {
                hits++;
            }

            internal void WriteInformation(Action<string> writeLine)
            {
                writeLine($"{IntegrationId}:{hits}");
            }

            public static HitTrace Parse(string line)
            {
                var sections = line.Split(':').ToArray();
                var id = int.Parse(sections[0]);
                var currentCounter = int.Parse(sections[1]);
                return new HitTrace(id, currentCounter);
            }
        }
    }
}
