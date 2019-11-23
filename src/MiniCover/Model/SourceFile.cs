using System;
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
                Instructions = sources.SelectMany(s => s.Instructions).ToList()
            };
        }

        public List<InstrumentedInstruction> Instructions = new List<InstrumentedInstruction>();

        internal object SelectMany(Func<object, object> p)
        {
            throw new NotImplementedException();
        }
    }
}
