using System.Collections.Concurrent;
using System.Collections.Generic;
using Mono.Cecil.Cil;

namespace MiniCover.Instrumentation
{
    public class SequencePointMap
    {
        private ConcurrentDictionary<SequencePoint, List<Instruction>> _instructionsBySequencePoint;
        private ConcurrentDictionary<Instruction, SequencePoint> _sequencePointByInstruction;

        public IEnumerable<SequencePoint> SequencePoints => _instructionsBySequencePoint.Keys;

        public IEnumerable<Instruction> GetInstructions(SequencePoint sequencePoint)
        {
            if (!_instructionsBySequencePoint.TryGetValue(sequencePoint, out var i))
                return null;

            return i;
        }

        public SequencePoint GetSequencePoint(Instruction instruction)
        {
            if (!_sequencePointByInstruction.TryGetValue(instruction, out var s))
                return null;

            return s;
        }

        public void Add(SequencePoint sequencePoint, Instruction instruction)
        {
            var instructions = _instructionsBySequencePoint.GetOrAdd(sequencePoint, (s) => new List<Instruction>());
            instructions.Add(instruction);

            _sequencePointByInstruction.TryAdd(instruction, sequencePoint);
        }
    }
}
