using System.Collections.Generic;
using System.Linq;

namespace MiniCover.Model
{
    public class SourceFile
    {
        public static SourceFile Merge(IEnumerable<SourceFile> sources)
        {
            return new SourceFile
            {
                Sequences = sources.SelectMany(s => s.Sequences).ToList()
            };
        }

        public List<InstrumentedSequence> Sequences = new List<InstrumentedSequence>();
    }
}
