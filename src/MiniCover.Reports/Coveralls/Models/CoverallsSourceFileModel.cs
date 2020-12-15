using Newtonsoft.Json;

namespace MiniCover.Reports.Coveralls.Models
{
    class CoverallsSourceFileModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("source_digest")]
        public string SourceDigest { get; set; }

        [JsonProperty("coverage")]
        public int?[] Coverage { get; set; }
    }
}