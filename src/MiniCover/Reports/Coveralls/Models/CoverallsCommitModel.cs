using Newtonsoft.Json;

namespace MiniCover.Reports.Coveralls.Models
{
    class CoverallsCommitModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("author_name")]
        public string AuthorName { get; set; }

        [JsonProperty("author_email")]
        public string AuthorEmail { get; set; }

        [JsonProperty("committer_name")]
        public string CommitterName { get; set; }

        [JsonProperty("committer_email")]
        public string CommitterEmail { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}