using System.Collections.Generic;
using Newtonsoft.Json;

namespace MiniCover.Model
{
    public class InstrumentedInstruction
    {
        public int Id { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public int StartColumn { get; set; }
        public int EndColumn { get; set; }
        public string Instruction { get; set; }
        public InstrumentedMethod Method { get; set; }

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
