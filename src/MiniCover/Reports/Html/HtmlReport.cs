using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Text.RegularExpressions;
using MiniCover.Model;
using MiniCover.Reports.Helpers;
using MiniCover.Utils;

namespace MiniCover.Reports.Html
{
    public class HtmlReport : IHtmlReport
    {
        public virtual int Execute(InstrumentationResult result, IDirectoryInfo output, float threshold)
        {
            Directory.CreateDirectory(output.FullName);

            var hitsInfo = HitsInfo.TryReadFromDirectory(result.HitsPath);

            var fileName = Path.Combine(output.FullName, "index.html");

            var sourceFiles = result.GetSourceFiles();

            var totalLines = sourceFiles.Sum(sf =>
                sf.Sequences
                    .SelectMany(s => s.GetLines())
                    .Distinct()
                    .Count()
            );

            var totalCoveredLines = sourceFiles.Sum(sf =>
                sf.Sequences
                    .Where(s => hitsInfo.WasHit(s.HitId))
                    .SelectMany(s => s.GetLines())
                    .Distinct()
                    .Count()
            );

            var totalCoveragePercentage = (float)totalCoveredLines / totalLines;
            var isHigherThanThreshold = totalCoveragePercentage >= threshold;
            var totalThresholdClass = isHigherThanThreshold ? "green" : "red";

            using (var htmlWriter = (TextWriter)File.CreateText(fileName))
            {
                htmlWriter.WriteLine("<html>");
                htmlWriter.WriteLine("<style>");
                htmlWriter.WriteLine(ResourceUtils.GetContent("MiniCover.Reports.Html.Shared.css"));
                htmlWriter.WriteLine(ResourceUtils.GetContent("MiniCover.Reports.Html.Summary.css"));
                htmlWriter.WriteLine("</style>");
                htmlWriter.WriteLine("<script>");
                htmlWriter.WriteLine(ResourceUtils.GetContent("MiniCover.Reports.Html.Shared.js"));

                htmlWriter.WriteLine("</script>");

                htmlWriter.WriteLine("<body>");

                // Write summary
                htmlWriter.WriteLine("<h2>Summary</h2>");
                htmlWriter.WriteLine("<table>");
                htmlWriter.WriteLine($"<tr><th>Generated on</th><td>{DateTime.Now}</td></tr>");
                htmlWriter.WriteLine($"<tr><th>Threshold</th><td>{threshold:P}</td></tr>");
                htmlWriter.WriteLine("</table>");

                // Write detailed report
                htmlWriter.WriteLine("<h2>Source Files</h2>");
                htmlWriter.WriteLine("<table border=\"1\" cellpadding=\"5\">");
                htmlWriter.WriteLine("<tr>");
                htmlWriter.WriteLine("<th>File</th>");
                htmlWriter.WriteLine("<th class=\"value\">Lines</th>");
                htmlWriter.WriteLine("<th class=\"value\">% Lines</th>");
                htmlWriter.WriteLine("<th class=\"value\">Stmts</th>");
                htmlWriter.WriteLine("<th class=\"value\">% Stmts</th>");
                htmlWriter.WriteLine("<th class=\"value\">Branches</th>");
                htmlWriter.WriteLine("<th class=\"value\">% Branches</th>");
                htmlWriter.WriteLine("</tr>");

                foreach (var summaryRow in SummaryFactory.GetSummaryGrid(result.GetSourceFiles(), hitsInfo, threshold))
                {
                    var summary = summaryRow.Summary;

                    var statementsCoverageClass = summary.StatementsCoveragePass ? "green" : "red";
                    var linesCoverageClass = summary.LinesCoveragePass ? "green" : "red";
                    var branchesCoverageClass = summary.BranchesCoveragePass ? "green" : "red";

                    var classes = new List<string> { };

                    if (summaryRow.Level == 0)
                        classes.Add("root");
                    if (summaryRow.Folder)
                        classes.Add("folder");
                    if (summaryRow.File)
                        classes.Add("file");

                    var marginLeft = Math.Max(summaryRow.Level - 1, 0) * 20;

                    htmlWriter.WriteLine($"<tr class=\"{string.Join(" ", classes)}\">");
                    htmlWriter.WriteLine($"<td>");
                    if (summaryRow.SourceFiles.Length == 1)
                    {
                        var indexRelativeFileName = GetIndexRelativeHtmlFileName(summaryRow.SourceFiles[0].Path);
                        htmlWriter.WriteLine($"<a class=\"name\" href=\"{indexRelativeFileName}\" style=\"margin-left: {marginLeft}px\">{summaryRow.Name}</a>");
                    }
                    else
                    {
                        htmlWriter.WriteLine($"<span class=\"name\" style=\"margin-left: {marginLeft}px\">{summaryRow.Name}</span");
                    }
                    htmlWriter.WriteLine("</td>");
                    htmlWriter.WriteLine($"<td class=\"value {linesCoverageClass}\">{summary.CoveredLines} / {summary.Lines}</td>");
                    htmlWriter.WriteLine($"<td class=\"value {linesCoverageClass}\">{summary.LinesPercentage:P}</td>");
                    htmlWriter.WriteLine($"<td class=\"value {statementsCoverageClass}\">{summary.CoveredStatements} / {summary.Statements}</td>");
                    htmlWriter.WriteLine($"<td class=\"value {statementsCoverageClass}\">{summary.StatementsPercentage:P}</td>");
                    htmlWriter.WriteLine($"<td class=\"value {branchesCoverageClass}\">{summary.CoveredBranches} / {summary.Branches}</td>");
                    htmlWriter.WriteLine($"<td class=\"value {branchesCoverageClass}\">{summary.BranchesPercentage:P}</td>");
                    htmlWriter.WriteLine("</tr>");

                    if (summaryRow.SourceFiles.Length == 1)
                    {
                        var relativeFileName = GetHtmlFileName(output, summaryRow.SourceFiles[0].Path);

                        new HtmlSourceFileReport()
                            .Generate(result, summaryRow.SourceFiles.First(), hitsInfo, threshold, relativeFileName);
                    }
                }

                htmlWriter.WriteLine("</table>");
                htmlWriter.WriteLine("</body>");
                htmlWriter.WriteLine("</html>");
            }

            return isHigherThanThreshold ? 0 : 1;
        }

        private string GetIndexRelativeHtmlFileName(string fileName)
        {
            string safeName = Regex.Replace(fileName, @"^[./\\]+", "");
            return safeName + ".html";
        }

        private string GetHtmlFileName(IDirectoryInfo output, string fileName)
        {
            string indexRelativeFileName = GetIndexRelativeHtmlFileName(fileName);
            return Path.Combine(output.FullName, indexRelativeFileName);
        }
    }
}
