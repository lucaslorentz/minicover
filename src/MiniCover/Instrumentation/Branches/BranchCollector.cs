using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Mono.Cecil.Cil;

namespace MiniCover.Instrumentation.Branches
{
    public static class BranchCollector
    {
        public static IList<BranchInfo> Collect(
            Instruction instruction,
            HashSet<Instruction> codeInstructions)
        {
            var branches = new List<BranchInfo>();
            Visit(instruction, codeInstructions, ImmutableHashSet<Instruction>.Empty, branches);
            return branches.Distinct().ToArray();
        }

        private static IEnumerable<Instruction> Visit(
            Instruction instruction,
            HashSet<Instruction> codeInstructions,
            ImmutableHashSet<Instruction> visitedInstructions,
            List<BranchInfo> branches)
        {
            if (instruction == null || visitedInstructions.Contains(instruction))
                return Enumerable.Empty<Instruction>();

            var newVisitedInstructions = visitedInstructions.Add(instruction);

            var targets = GetTargets(instruction, codeInstructions, newVisitedInstructions, branches)
                .Distinct()
                .ToArray();

            if (!codeInstructions.Contains(instruction))
                return targets;

            if (targets.Length > 1)
            {
                branches.Add(new BranchInfo
                {
                    PivotInstruction = instruction,
                    Targets = targets
                });
            }

            return new Instruction[] { instruction };
        }

        private static IEnumerable<Instruction> GetTargets(
                Instruction instruction,
                HashSet<Instruction> codeInstructions,
                ImmutableHashSet<Instruction> visitedInstructions,
                List<BranchInfo> branches)
        {
            if (instruction.OpCode.FlowControl == FlowControl.Cond_Branch)
            {
                var next = Visit(instruction.Next, codeInstructions, visitedInstructions, branches);

                if (instruction.Operand is Instruction[] operands)
                    return next.Concat(operands.SelectMany(o =>
                        Visit(o, codeInstructions, visitedInstructions, branches)));

                if (instruction.Operand is Instruction operand)
                    return next.Concat(
                        Visit(operand, codeInstructions, visitedInstructions, branches));
            }

            if (instruction.OpCode.FlowControl == FlowControl.Branch)
                return Visit(instruction.Operand as Instruction, codeInstructions, visitedInstructions, branches);

            return Visit(instruction.Next, codeInstructions, visitedInstructions, branches);
        }
    }
}
