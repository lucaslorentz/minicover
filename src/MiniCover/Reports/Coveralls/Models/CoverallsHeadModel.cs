using Newtonsoft.Json;

namespace MiniCover.Reports.Coveralls.Models
{
    class CoverallsHeadModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}