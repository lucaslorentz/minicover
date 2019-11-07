using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using MiniCover.Model;

namespace MiniCover.Reports
{
    public class HtmlReport : BaseReport
    {
        private const string BgColorGreen = "background-color: #D2EACE;";
        private const string BgColorRed = "background-color: #EACECC;";
        private const string BgColorNeutral = "background-color: #EEF4ED;";
        private const string CursorPointer = "cursor: pointer;";
        private readonly string _output;
        private readonly StringBuilder _htmlReport;

        public HtmlReport(string output)
        {
            _output = output;
            _htmlReport = new StringBuilder();
        }

        protected override void SetFileColumnLength(int fileColumnsLength)
        {
        }

        protected override void WriteHeader()
        {
        }

        protected override void WriteReport(KeyValuePair<string, SourceFile> kvFile, int lines, int coveredLines, float coveragePercentage, ConsoleColor color)
        {
            var indexRelativeFileName = GetIndexRelativeHtmlFileName(kvFile.Key);
            _htmlReport.AppendLine("<tr>");
            _htmlReport.AppendLine($"<td><a href=\"{indexRelativeFileName}\">{indexRelativeFileName}</a></td>");
            _htmlReport.AppendLine($"<td>{lines}</td>");
            _htmlReport.AppendLine($"<td>{coveredLines}</td>");
            _htmlReport.AppendLine($"<td style=\"{GetBgColor(color)}\">{coveragePercentage:P}</td>");
            _htmlReport.AppendLine("</tr>");
        }

        protected override void WriteDetailedReport(InstrumentationResult result, IDictionary<string, SourceFile> files, HitsInfo hitsInfo)
        {
            foreach (var kvFile in files)
            {
                var lines = File.ReadAllLines(Path.Combine(result.SourcePath, kvFile.Key));

                var fileName = GetHtmlFileName(kvFile.Key);

                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                using (var htmlWriter = (TextWriter)File.CreateText(fileName))
                {
                    htmlWriter.WriteLine("<html>");
                    htmlWriter.WriteLine("<style>");
                    htmlWriter.WriteLine("details summary::-webkit-details-marker {");
                    htmlWriter.WriteLine("display: none;");
                    htmlWriter.WriteLine("}");
                    htmlWriter.WriteLine("</style>");
                    htmlWriter.WriteLine("<body style=\"font-family: monospace;\">");

                    var uncoveredLineNumbers = new HashSet<int>();
                    var coveredLineNumbers = new HashSet<int>();
                    foreach (var i in kvFile.Value.Instructions)
                    {
                        if (hitsInfo.IsInstructionHit(i.Id))
                        {
                            coveredLineNumbers.UnionWith(i.GetLines());
                        }
                        else
                        {
                            uncoveredLineNumbers.UnionWith(i.GetLines());
                        }
                    }

                    var l = 0;
                    foreach (var line in lines)
                    {
                        l++;
                        var style = "white-space: pre;";
                        if (coveredLineNumbers.Contains(l))
                        {
                            style += BgColorGreen;
                            style += CursorPointer;
                        }
                        else if (uncoveredLineNumbers.Contains(l))
                        {
                            style += BgColorRed;
                        }
                        else
                        {
                            style += BgColorNeutral;
                        }

                        var instructions = kvFile.Value.Instructions
                            .Where(i => i.GetLines().Contains(l))
                            .ToArray();

                        var counter = instructions.Sum(a => hitsInfo.GetInstructionHitCount(a.Id));

                        var codeContent = !string.IsNullOrEmpty(line)
                            ? WebUtility.HtmlEncode(line)
                            : "&nbsp;";

                        var contexts = instructions
                            .SelectMany(i => hitsInfo.GetInstructionHitContexts(i.Id))
                            .Distinct()
                            .ToArray();

                        var hitCountHtml = coveredLineNumbers.Contains(l) || uncoveredLineNumbers.Contains(l)
                            ? $"<span style=\"display: inline-block; width: 30px; font-size: 10px;\">{counter}x</span>"
                            : "<span style=\"display: inline-block; width: 30px; font-size: 10px;\"></span>";

                        if (coveredLineNumbers.Contains(l))
                        {
                            htmlWriter.WriteLine($"<details>");
                            htmlWriter.WriteLine($"<summary style=\"{style}\">{hitCountHtml}{codeContent}</summary>");

                            htmlWriter.WriteLine("<ul>");
                            foreach (var context in contexts)
                            {
                                var count = instructions.Sum(i => context.GetHitCount(i.Id));
                                var description = $"{context.ClassName}.{context.MethodName}";
                                htmlWriter.WriteLine($"<li>{WebUtility.HtmlEncode(description)}: {count}x</li>");
                            }
                            htmlWriter.WriteLine("</ul>");

                            htmlWriter.WriteLine($"</details>");
                        }
                        else
                        {
                            htmlWriter.WriteLine($"<div style=\"{style}\">{hitCountHtml}{codeContent}</div>");
                        }
                    }

                    htmlWriter.WriteLine("</body>");
                    htmlWriter.WriteLine("</html>");
                }
            }
        }

        protected override void WriteFooter(int lines, int coveredLines, float coveragePercentage, float threshold, ConsoleColor color)
        {
            var result = new StringBuilder();

            result.AppendLine("<html>");
            result.AppendLine("<body style=\"font-family: sans-serif;\">");

            // Write summary
            result.AppendLine("<h1>Summary</h1>");
            result.AppendLine("<table border=\"1\" cellpadding=\"5\">");
            result.AppendLine($"<tr><th>Generated on</th><td>{DateTime.Now}</td></tr>");
            result.AppendLine($"<tr><th>Lines</th><td>{lines}</td></tr>");
            result.AppendLine($"<tr><th>Covered Lines</th><td>{coveredLines}</td></tr>");
            result.AppendLine($"<tr><th>Threshold</th><td>{threshold:P}</td></tr>");
            result.AppendLine($"<tr><th>Percentage</th><td style=\"{GetBgColor(color)}\">{coveragePercentage:P}</td></tr>");
            result.AppendLine("</table>");

            // Write detailed report
            result.AppendLine("<h1>Coverage</h1>");
            result.AppendLine("<table border=\"1\" cellpadding=\"5\">");
            result.AppendLine("<tr>");
            result.AppendLine("<th>File</th>");
            result.AppendLine("<th>Lines</th>");
            result.AppendLine("<th>Covered Lines</th>");
            result.AppendLine("<th>Percentage</th>");
            result.AppendLine("</tr>");
            result.Append(_htmlReport);
            result.AppendLine("</table>");
            result.AppendLine("</body>");
            result.AppendLine("</html>");

            Directory.CreateDirectory(_output);
            var fileName = Path.Combine(_output, "index.html");
            using (var htmlWriter = (TextWriter)File.CreateText(fileName))
            {
                htmlWriter.WriteLine(result.ToString());
            }
        }

        private string GetBgColor(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Green:
                    return BgColorGreen;
                case ConsoleColor.Red:
                    return BgColorRed;
                default:
                    throw new ArgumentException($"Invalid color: {color}");
            }
        }

        private string GetIndexRelativeHtmlFileName(string fileName)
        {
            string safeName = Regex.Replace(fileName, @"^[./\\]+", "");
            return safeName + ".html";
        }

        private string GetHtmlFileName(string fileName)
        {
            string indexRelativeFileName = GetIndexRelativeHtmlFileName(fileName);
            return Path.Combine(_output, indexRelativeFileName);
        }
    }
}
