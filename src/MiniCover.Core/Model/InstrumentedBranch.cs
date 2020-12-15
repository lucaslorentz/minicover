using Newtonsoft.Json;

namespace MiniCover.Core.Model
{
    public class InstrumentedBranch
    {
        public int HitId { get; set; }
        public bool External { get; set; }

        [JsonIgnore]
        public string Instruction { get; set; }
    }
}
