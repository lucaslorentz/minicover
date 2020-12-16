using System.Collections.Generic;
using Newtonsoft.Json;

namespace MiniCover.Core.Model
{
    public class InstrumentedSequence
    {
        public int HitId { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public int StartColumn { get; set; }
        public int EndColumn { get; set; }
        public InstrumentedMethod Method { get; set; }
        public InstrumentedCondition[] Conditions { get; set; } = new InstrumentedCondition[0];

        [JsonIgnore]
        public string Instruction { get; set; }

        [JsonIgnore]
        public string Code { get; set; }

        public IEnumerable<int> GetLines()
        {
            for (var lineIndex = StartLine; lineIndex <= EndLine; lineIndex++)
            {
                yield return lineIndex;
            }
        }
    }
}
