using System;
using System.Collections.Generic;
using MiniCover.Model;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Security.Cryptography;
using System.Net.Http;
using Newtonsoft.Json;

namespace MiniCover.Reports
{

    class CoverallLine
    {
        public int InstructionId = -1;
        public int InstructionLineCounts = int.MaxValue;
        public int Hits;
    }

    public class CoverallsReport : BaseReport
    {

        private const string BgColorGreen = "background-color: #D2EACE;";
        private const string BgColorRed = "background-color: #EACECC;";
        private const string BgColorBlue = "background-color: #EEF4ED;";

        private readonly string _output;
        private readonly string _job_id;
        private readonly string _repo_token;
        private readonly string _service_name;
        private readonly string _message;
        private readonly string _rootfolder;
        private readonly string _post;
        private readonly string _git_id;
        private readonly string _git_branch;
        private readonly string _git_remote_name;
        private readonly string _git_remote_url;
        //private readonly float  _threshold;

        public CoverallsReport(string output
            , string repo_token
            , string job_id
            , string service_name
            , string message
            , string rootfolder
            , string post
            , string git_id
            , string git_branch
            , string git_remote_name
            , string git_remote_url
            //, float threshold
            )
        {
            _output = output;
            _repo_token = repo_token;
            _job_id = job_id;
            _service_name = (service_name ?? "local");
            _rootfolder = rootfolder;
            _post = post;
            _git_id = git_id;
            _git_branch = (git_branch ?? "master");
            _git_remote_name = git_remote_name;
            _git_remote_url = git_remote_url;
            _message = (message ?? "--");
            //_threshold = threshold;

            if(null == _repo_token)
                throw new ArgumentException("token not specified");

            if(null == _job_id)
                throw new ArgumentException("job id not specified");

            if(null == _git_id)
                throw new ArgumentException("git id not specified");

            if(null == _git_remote_name)
                throw new ArgumentException("git remote name not specified");

            if(null == _git_remote_url)
                throw new ArgumentException("git remote name not specified");

        }

        private void WriteWarning(string Warning)
        {
            ConsoleColor original = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Warning: ");
            Console.WriteLine(Warning);
            Console.ForegroundColor = original;
        }

        
        private bool Upload(string fileData)
        {
            using (HttpContent stringContent = new StringContent(fileData))
            {
                using (var client = new HttpClient())
                using (var formData = new MultipartFormDataContent())
                {
                    formData.Add(stringContent, "json_file", "coverage.json");

                    var response = client.PostAsync(_post, formData).Result;

                    if (!response.IsSuccessStatusCode)
                    {
                        var content = response.Content.ReadAsStringAsync().Result;
                        dynamic result = JsonConvert.DeserializeObject(content);
                        var message = result.message;

                        /*
                        if (message.Length > 200)
                        {
                            message = message.Substring(0, 200);
                        }
                        */

                        System.Console.WriteLine( string.Format("{0} - {1}", result.StatusCode, message));

                        return false;
                    }

                    return true;
                }
            }
        }


        protected override void SetFileColumnLength(int fileColumnsLength)
        {
        }

        protected override void WriteHeader()
        {
        }



        protected override void WriteReport(KeyValuePair<string, SourceFile> kvFile, int lines, int coveredLines, float coveragePercentage, ConsoleColor color)
        {

        }

        protected override void WriteDetailedReport(InstrumentationResult result, IDictionary<string, SourceFile> files, Hits hits)
        {

            StringBuilder json = new StringBuilder();

            json.Append($@"{{ 
                                  ""service_name"": ""{_service_name}""
                                , ""service_job_id"": ""{_job_id}""
                                , ""repo_token"" : ""{_repo_token}""
                                , ""git"": {{
                                        ""head"": {{
                                                  ""id"": ""{_git_id}""
                                                , ""message"": ""{_message}""
                                                }}
                                            , ""branch"": ""{_git_branch}""
                                            , ""remotes"": [
                                                {{
                                                  ""name"": ""{_git_remote_name}"" 
                                                   , ""url"": ""{_git_remote_url}""
                                                }}
                                              ]
                                    }}
                                , ""source_files"": [
                           "
                      );


            double all_count = 0;
            double all_total = 0;

            foreach (var kvFile in files)
            {
                var SourceFile  = Path.Combine(result.SourcePath, kvFile.Key);
                var SourceLines = File.ReadAllLines(SourceFile);

                //var fileName = kvFile.Key;

                CoverallLine[] lines = new CoverallLine[SourceLines.Length];
                
                foreach (var instruction in kvFile.Value.Instructions)
                {                    
                    int InstructionLineCounts = instruction.EndLine - instruction.StartLine;
                    for (int l = instruction.StartLine; l <= instruction.EndLine; l++)
                    {

                        CoverallLine line = lines[(l - 1)];

                        if (null == line)
                        {
                            lines[(l - 1)] = line = new CoverallLine();

                        }

                        if (-1 != line.InstructionId)
                        {
                            WriteWarning($"Duplicated instruction {line.InstructionId} and {instruction.Id} both cover line {l}");
                        }

                        if (InstructionLineCounts < line.InstructionLineCounts)
                        {
                            line.InstructionId = instruction.Id;
                            line.Hits = hits.GetInstructionHitCount(instruction.Id);
                            line.InstructionLineCounts = InstructionLineCounts;
                        }
                    }
                }

                var fileName = Path.GetRelativePath(_rootfolder, SourceFile).Replace("\\", "/");

                using (var md5 = MD5.Create())
                {
                    using (var stream = File.OpenRead(SourceFile))
                    {
                        var hash = md5.ComputeHash(stream);

                        json.Append( $@"    {{ 
                                            ""name"": ""{fileName}""
                                          , ""source_digest"": ""{System.Convert.ToBase64String(hash)}""
                                          , ""coverage"": [
                           "
                      );
                    }
                }
                
                string join = "";
                double count = 0;
                double total = 0;
                foreach (var l in lines)
                {
                    json.Append(join);
                    join = ", ";
                    string hit = (l == null || 0 > l.Hits)? "null" : l.Hits.ToString();
                    
                    if("null" != hit && 0 < l.Hits)
                        count++;

                    if(null != l)
                        total++;

                    json.Append( hit );
                    
                }

                json.Append($"]}}");

                System.Console.WriteLine($"Coverage {fileName} {count / total} ");

                all_count += count;
                all_total += total;
            }

            json.Append($"]}}");

            if(null != _output)
                    File.WriteAllText(_output, json.ToString());
                else
                    System.Console.WriteLine(json.ToString());

            System.Console.WriteLine($"Coverage total {all_count / all_total} ");

            if (null != _post)
            {
                bool coverall = Upload(json.ToString());
                if(!coverall)
                    throw new ApplicationException("Caverall Failed");
            }
               
        }

        protected override void WriteFooter(int lines, int coveredLines, float coveragePercentage, float threshold, ConsoleColor color)
        {
            
        }

        
    }
}
