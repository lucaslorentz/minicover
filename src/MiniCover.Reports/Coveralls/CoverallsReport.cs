using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using MiniCover.Core.Extensions;
using MiniCover.Core.Hits;
using MiniCover.Core.Model;
using MiniCover.Core.Utils;
using MiniCover.Reports.Coveralls.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace MiniCover.Reports.Coveralls
{
    public class CoverallsReport : ICoverallsReport
    {
        private const string coverallsJobsUrl = "https://coveralls.io/api/v1/jobs";

        private readonly IHitsReader _hitsReader;
        private readonly IFileSystem _fileSystem;

        public CoverallsReport(
            IHitsReader hitsReader,
            IFileSystem fileSystem)
        {
            _hitsReader = hitsReader;
            _fileSystem = fileSystem;
        }

        public virtual async Task<int> Execute(InstrumentationResult result,
            string output,
            string repoToken,
            string serviceJobId,
            string serviceName,
            string commitMessage,
            string rootFolder,
            string commit,
            string commitAuthorName,
            string commitAuthorEmail,
            string commitCommitterName,
            string commitCommitterEmail,
            string branch,
            string remoteName,
            string remoteUrl)
        {
            var hits = _hitsReader.TryReadFromDirectory(result.HitsPath);

            var files = result.GetSourceFiles();

            var coverallsJob = new CoverallsJobModel
            {
                ServiceJobId = serviceJobId,
                ServiceName = serviceName,
                RepoToken = repoToken,
                CoverallsGitModel = !string.IsNullOrWhiteSpace(branch)
                    ? new CoverallsGitModel
                    {
                        Head = !string.IsNullOrWhiteSpace(commit)
                            ? new CoverallsCommitModel
                            {
                                Id = commit,
                                AuthorName = commitAuthorName,
                                AuthorEmail = commitAuthorEmail,
                                CommitterName = commitCommitterName,
                                CommitterEmail = commitCommitterEmail,
                                Message = commitMessage
                            }
                            : null,
                        Branch = branch,
                        Remotes = !string.IsNullOrWhiteSpace(remoteUrl)
                            ? new List<CoverallsRemote>
                            {
                                new CoverallsRemote
                                {
                                    Name = remoteName,
                                    Url = remoteUrl
                                }
                            }
                            : null
                    }
                    : null,
                SourceFiles = new List<CoverallsSourceFileModel>()
            };

            foreach (var file in files)
            {
                var sourceFile = Path.Combine(result.SourcePath, file.Path);

                if (!_fileSystem.File.Exists(sourceFile))
                {
                    System.Console.WriteLine($"File not found: {sourceFile}");
                    continue;
                }

                var sourceLines = _fileSystem.File.ReadAllLines(sourceFile);

                var hitsPerLine = file.Sequences
                    .GroupByMany(f => f.GetLines())
                    .ToDictionary(g => g.Key, g => g.Sum(i => hits.GetHitCount(i.HitId)));

                var fileName = PathUtils.GetRelativePath(rootFolder, sourceFile).Replace("\\", "/");

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

            var coverallsJson = JsonConvert.SerializeObject(coverallsJob, Formatting.None, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });

            if (!string.IsNullOrWhiteSpace(output))
            {
                _fileSystem.File.WriteAllText(output, coverallsJson);
            }

            return await Post(coverallsJson);
        }

        private string ComputeSourceDigest(string sourceFile)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = _fileSystem.File.OpenRead(sourceFile))
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
                        System.Console.Error.Write($"Coveralls upload failed: {response.StatusCode} - {message}");
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
