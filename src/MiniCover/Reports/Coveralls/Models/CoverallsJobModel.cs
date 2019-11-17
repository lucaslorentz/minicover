using Newtonsoft.Json;
using System.Collections.Generic;

namespace MiniCover.Reports.Coveralls.Models
{
    class CoverallsJobModel
    {
        [JsonProperty("service_job_id")]
        public string ServiceJobId { get; set; }

        [JsonProperty("service_name")]
        public string ServiceName { get; set; }

        [JsonProperty("repo_token")]
        public string RepoToken { get; set; }

        [JsonProperty("git")]
        public CoverallsGitModel CoverallsGitModel { get; set; }

        [JsonProperty("source_files")]
        public List<CoverallsSourceFileModel> SourceFiles { get; set; }
    }
}