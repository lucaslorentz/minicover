using MiniCover.Model;
using MiniCover.Reports.Coveralls.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MiniCover.Reports.Coveralls
{
    public class CoverallsReport
    {
        private const string coverallsJobsUrl = "https://coveralls.io/api/v1/jobs";

        private readonly string _output;
        private readonly string _repoToken;
        private readonly string _serviceJobId;
        private readonly string _serviceName;
        private readonly string _commitMessage;
        private readonly string _rootFolder;
        private readonly string _commit;
        private readonly string _commitAuthorName;
        private readonly string _commitAuthorEmail;
        private readonly string _commitCommitterName;
        private readonly string _commitCommitterEmail;
        private readonly string _branch;
        private readonly string _remoteName;
        private readonly string _remoteUrl;

        public CoverallsReport(
            string output,
            string repoToken,
            string serviceJobId,
            string serviceName,
            string commitMessage,
            string rootfolder,
            string commit,
            string commitAuthorName,
            string commitAuthorEmail,
            string commitCommitterName,
            string commitCommitterEmail,
            string branch,
            string remoteName,
            string remoteUrl)
        {
            _output = output;
            _repoToken = repoToken;
            _serviceJobId = serviceJobId;
            _serviceName = serviceName;
            _rootFolder = rootfolder;
            _commit = commit;
            _commitAuthorName = commitAuthorName;
            _commitAuthorEmail = commitAuthorEmail;
            _commitCommitterName = commitCommitterName;
            _commitCommitterEmail = commitCommitterEmail;
            _branch = branch;
            _remoteName = remoteName;
            _remoteUrl = remoteUrl;
            _commitMessage = commitMessage;
        }

        public virtual async Task<int> Execute(InstrumentationResult result)
        {
            var hits = HitsInfo.TryReadFromDirectory(result.HitsPath);

            var files = result.GetSourceFiles();

            var coverallsJob = new CoverallsJobModel
            {
                ServiceJobId = _serviceJobId,
                ServiceName = _serviceName,
                RepoToken = _repoToken,
                CoverallsGitModel = !string.IsNullOrWhiteSpace(_branch)
                    ? new CoverallsGitModel
                    {
                        Head = !string.IsNullOrWhiteSpace(_commit)
                            ? new CoverallsCommitModel
                            {
                                Id = _commit,
                                AuthorName = _commitAuthorName,
                                AuthorEmail = _commitAuthorEmail,
                                CommitterName = _commitCommitterName,
                                CommitterEmail = _commitCommitterEmail,
                                Message = _commitMessage
                            }
                            : null,
                        Branch = _branch,
                        Remotes = !string.IsNullOrWhiteSpace(_remoteUrl)
                            ? new List<CoverallsRemote>
                            {
                                new CoverallsRemote
                                {
                                    Name = _remoteName,
                                    Url = _remoteUrl
                                }
                            }
                            : null
                    }
                    : null,
                SourceFiles = new List<CoverallsSourceFileModel>()
            };

            foreach (var kvFile in files)
            {
                var sourceFile = Path.Combine(result.SourcePath, kvFile.Key);

                if (!File.Exists(sourceFile))
                {
                    Console.WriteLine($"File not found: {sourceFile}");
                    continue;
                }

                var sourceLines = File.ReadAllLines(sourceFile);

                var hitsPerLine = kvFile.Value.Instructions
                    .SelectMany(i => i.GetLines(), (instruction, line) => new { instruction, line })
                    .GroupBy(i => i.line)
                    .ToDictionary(g => g.Key, g => g.Sum(j => hits.GetInstructionHitCount(j.instruction.Id)));

                var fileName = Path.GetRelativePath(_rootFolder, sourceFile).Replace("\\", "/");

                var coverallsSourceFileModel = new CoverallsSourceFileModel
                {
                    Name = fileName,
                    SourceDigest = ComputeSourceDigest(sourceFile),
                    Coverage = Enumerable.Range(1, sourceLines.Length).Select(line =>
                    {
                        return hitsPerLine.ContainsKey(line)
                            ? hitsPerLine[line]
                            : default(int?);
                    }).ToArray()
                };

                coverallsJob.SourceFiles.Add(coverallsSourceFileModel);
            }

            var coverallsJson = JsonConvert.SerializeObject(coverallsJob, Formatting.None, new JsonSerializerSettings {
                NullValueHandling = NullValueHandling.Ignore
            });

            if (!string.IsNullOrWhiteSpace(_output))
            {
                File.WriteAllText(_output, coverallsJson);
            }

            return await Post(coverallsJson);
        }

        private static string ComputeSourceDigest(string sourceFile)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(sourceFile))
                {
                    var hash = md5.ComputeHash(stream);
                    return Convert.ToBase64String(hash);
                }
            }
        }

        private async Task<int> Post(string json)
        {
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            using (var client = new HttpClient())
            {
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(stringContent, "json_file", "coverage.json");

                    var response = await client.PostAsync(coverallsJobsUrl, formData);

                    if (!response.IsSuccessStatusCode)
                    {
                        var message = await GetErrorMessage(response);
                        Console.Error.Write($"Coveralls upload failed: {response.StatusCode} - {message}");
                        return 1;
                    }

                    return 0;
                }
            }
        }

        private static async Task<JToken> GetErrorMessage(HttpResponseMessage response)
        {
            if (response.Content != null)
            {
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    return responseContent;
                }
            }

            return response.ReasonPhrase;
        }
    }
}
