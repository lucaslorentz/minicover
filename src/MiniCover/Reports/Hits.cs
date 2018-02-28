using System.Collections.Generic;
using System.Linq;

namespace MiniCover.Reports
{
    public class Hits
    {
        private readonly IEnumerable<Hit> hits;

        private Hits(IEnumerable<Hit> hits)
        {
            this.hits = hits;
        }

        internal bool Contains(int id)
        {
            return this.hits.Any(hit => hit.InstrumentationId == id);
        }

        internal IEnumerable<Hit> ForMethod(int methodPointId)
        {
            return hits.Where(hit => hit.InstrumentationId.Equals(methodPointId)).ToArray();
        }

        internal static Hits Parse(string[] lines)
        {
            return new Hits(lines.Select(Hit.Parse));
        }
    }
}