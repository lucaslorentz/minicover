using Newtonsoft.Json;
using System.Collections.Generic;

namespace MiniCover.Reports.Coveralls.Models
{
    public class CoverallsGitModel
    {
        [JsonProperty("head")]
        public CoverallsCommitModel Head { get; set; }

        [JsonProperty("branch")]
        public string Branch { get; set; }

        [JsonProperty("remotes")]
        public List<CoverallsRemote> Remotes { get; set; }
    }
}