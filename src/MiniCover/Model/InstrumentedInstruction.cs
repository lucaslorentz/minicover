using System.Collections.Generic;

namespace MiniCover.Model
{
    public class InstrumentedInstruction
    {
        public int Id { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }
        public int StartColumn { get; set; }
        public int EndColumn { get; set; }
        public string Assembly { get; set; }
        public string Class { get; set; }
        public string Method { get; set; }
        public string MethodFullName { get; set; }
        public string Instruction { get; set; }

        public IEnumerable<int> GetLines()
        {
            for (var lineIndex = StartLine; lineIndex <= EndLine; lineIndex++)
            {
                yield return lineIndex;
            }
        }
    }
}
