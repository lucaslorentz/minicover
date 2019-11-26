using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil.Cil;

namespace MiniCover.Instrumentation.Branches
{
    public class BranchInfo : IEquatable<BranchInfo>
    {
        public Instruction PivotInstruction { get; set; }
        public Instruction[] Targets { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as BranchInfo);
        }

        public bool Equals(BranchInfo other)
        {
            return other != null &&
                   PivotInstruction == other.PivotInstruction &&
                   Targets.SequenceEqual(other.Targets);
        }

        public override int GetHashCode()
        {
            var hashcode = new HashCode();

            hashcode.Add(PivotInstruction);

            if (Targets != null)
            {
                for (var i = 0; i < Targets.Length; i++)
                    hashcode.Add(Targets[i]);
            }

            return hashcode.ToHashCode();
        }

        public static bool operator ==(BranchInfo left, BranchInfo right)
        {
            return EqualityComparer<BranchInfo>.Default.Equals(left, right);
        }

        public static bool operator !=(BranchInfo left, BranchInfo right)
        {
            return !(left == right);
        }
    }
}
