using System.Collections.Generic;
using System.Linq;

namespace MiniCover.Reports
{
    internal class Hits
    {
        private readonly IList<Hit> hits = new List<Hit>();

        internal Hits(string[] lines)
        {
            foreach (var line in lines)
            {
                var hit = Hit.Parse(line);
                this.hits.Add(hit);
            }
        }

        internal bool Contains(int id)
        {
            return this.hits.Any(hit => hit.InstrumentationId == id);
        }

        internal IEnumerable<Hit> ForMethod(int methodPointId)
        {
            return hits.Where(hit => hit.InstrumentationId.Equals(methodPointId)).ToArray();
        }
    }
}