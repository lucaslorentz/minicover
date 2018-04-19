using Newtonsoft.Json;

namespace MiniCover.Reports.Coveralls.Models
{
    class CoverallsRemote
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
