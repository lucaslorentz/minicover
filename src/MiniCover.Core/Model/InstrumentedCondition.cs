using System;
using Newtonsoft.Json;

namespace MiniCover.Core.Model
{
    public class InstrumentedCondition
    {
        public InstrumentedBranch[] Branches { get; set; } = new InstrumentedBranch[0];

        [JsonIgnore]
        public string Instruction { get; set; }
    }
}
