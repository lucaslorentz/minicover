using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace MiniCover.Model
{
    public class SourceFile
    {
        public static SourceFile Merge(IEnumerable<SourceFile> sources)
        {
            var path = sources.Select(s => s.Path).Distinct().Single();

            return new SourceFile(path)
            {
                Sequences = sources.SelectMany(s => s.Sequences).ToList()
            };
        }

        [JsonConstructor]
        public SourceFile(string path)
        {
            Path = path;
        }

        [JsonProperty(Order = -2)]
        public string Path { get; }

        public List<InstrumentedSequence> Sequences = new List<InstrumentedSequence>();
    }
}
