using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using MiniCover.Model;
using MiniCover.Utils;

namespace MiniCover.Reports.Html
{
    public class HtmlReport
    {
        private readonly string _output;

        public HtmlReport(string output)
        {
            _output = output;
        }

        public virtual int Execute(InstrumentationResult result, float threshold)
        {
            Directory.CreateDirectory(_output);

            var hitsInfo = HitsInfo.TryReadFromDirectory(result.HitsPath);

            var fileName = Path.Combine(_output, "index.html");

            var sourceFiles = result.GetSourceFiles();

            var totalLines = sourceFiles.Sum(sf =>
                sf.Value.Sequences
                    .SelectMany(s => s.GetLines())
                    .Distinct()
                    .Count()
            );

            var totalCoveredLines = sourceFiles.Sum(sf =>
                sf.Value.Sequences
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
                //htmlWriter.WriteLine($"<tr><th>Lines</th><td>{totalLines}</td></tr>");
                //htmlWriter.WriteLine($"<tr><th>Covered Lines</th><td>{totalCoveredLines}</td></tr>");
                htmlWriter.WriteLine($"<tr><th>Threshold</th><td>{threshold:P}</td></tr>");
                //htmlWriter.WriteLine($"<tr><th>Percentage</th><td class=\"{totalThresholdClass}\">{totalCoveragePercentage:P}</td></tr>");
                htmlWriter.WriteLine("</table>");

                // Write detailed report
                htmlWriter.WriteLine("<h2>Source Files</h2>");
                htmlWriter.WriteLine("<table border=\"1\" cellpadding =\"5\">");
                htmlWriter.WriteLine("<tr>");
                htmlWriter.WriteLine("<th>File</th>");
                htmlWriter.WriteLine("<th class=\"value\" colspan=\"2\">Lines</th>");
                htmlWriter.WriteLine("<th class=\"value\" colspan=\"2\">Statements</th>");
                htmlWriter.WriteLine("<th class=\"value\" colspan=\"2\">Branches</th>");
                htmlWriter.WriteLine("</tr>");

                foreach (var row in GetTableRows(result.GetSourceFiles()))
                {
                    var statements = row.SourceFiles.Sum(x =>
                        x.Sequences
                            .Count());

                    var coveredStatements = row.SourceFiles.Sum(x =>
                        x.Sequences
                            .Where(h => hitsInfo.WasHit(h.HitId))
                            .Count());

                    var lines = row.SourceFiles.Sum(x =>
                        x.Sequences
                            .SelectMany(s => s.GetLines())
                            .Distinct()
                            .Count());

                    var coveredLines = row.SourceFiles.Sum(x =>
                        x.Sequences
                            .Where(h => hitsInfo.WasHit(h.HitId))
                            .SelectMany(s => s.GetLines())
                            .Distinct()
                            .Count());

                    var branches = row.SourceFiles.Sum(x =>
                        x.Sequences
                            .SelectMany(s => s.Conditions)
                            .SelectMany(c => c.Branches)
                            .Count());

                    var coveredBranches = row.SourceFiles.Sum(x =>
                        x.Sequences
                            .SelectMany(s => s.Conditions)
                            .SelectMany(c => c.Branches)
                            .Where(b => hitsInfo.WasHit(b.HitId))
                            .Count());

                    var statementsPercentage = statements == 0 ? 1 : (float)coveredStatements / statements;
                    var statementsCoverageClass = statementsPercentage >= threshold ? "green" : "red";
                    var linesPercentage = lines == 0 ? 1 : (float)coveredLines / lines;
                    var linesCoverageClass = linesPercentage >= threshold ? "green" : "red";
                    var branchesPercentage = branches == 0 ? 1 : (float)coveredBranches / branches;
                    var branchesCoverageClass = branchesPercentage >= threshold ? "green" : "red";


                    htmlWriter.WriteLine($"<tr{(row.Class != null ? $" class=\"{row.Class}\"" : "")}>");
                    htmlWriter.WriteLine($"<td class=\"level-{row.Level} {linesCoverageClass}\">");
                    if (row.File != null)
                    {
                        var indexRelativeFileName = GetIndexRelativeHtmlFileName(row.File);
                        htmlWriter.WriteLine($"<a href=\"{indexRelativeFileName}\">{row.Name}</a>");
                    }
                    else
                    {
                        htmlWriter.WriteLine($"<span>{row.Name}</span");
                    }
                    htmlWriter.WriteLine("</td>");
                    htmlWriter.WriteLine($"<td class=\"value {linesCoverageClass}\">{coveredLines} / {lines}</td>");
                    htmlWriter.WriteLine($"<td class=\"value {linesCoverageClass}\">{linesPercentage:P}</td>");
                    htmlWriter.WriteLine($"<td class=\"value {statementsCoverageClass}\">{coveredStatements} / {statements}</td>");
                    htmlWriter.WriteLine($"<td class=\"value {statementsCoverageClass}\">{statementsPercentage:P}</td>");
                    htmlWriter.WriteLine($"<td class=\"value {branchesCoverageClass}\">{coveredBranches} / {branches}</td>");
                    htmlWriter.WriteLine($"<td class=\"value {branchesCoverageClass}\">{branchesPercentage:P}</td>");
                    htmlWriter.WriteLine("</tr>");

                    if (row.File != null)
                    {
                        new HtmlSourceFileReport()
                            .Generate(result, row.File, row.SourceFiles.First(), hitsInfo, threshold, GetHtmlFileName(row.File));
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

        private string GetHtmlFileName(string fileName)
        {
            string indexRelativeFileName = GetIndexRelativeHtmlFileName(fileName);
            return Path.Combine(_output, indexRelativeFileName);
        }

        private List<TableRow> GetTableRows(SortedDictionary<string, SourceFile> sourceFiles)
        {
            var tableRows = new List<TableRow>
            {
                new TableRow
                {
                    Level = 0,
                    Name = "Total",
                    File = null,
                    SourceFiles = sourceFiles.Values.ToArray(),
                    Class = "total"
                }
            };


            var items = sourceFiles
                .Select(kv => (
                    kv.Key.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }),
                    kv.Key,
                    kv.Value
                )).ToArray();

            AddRowsRecursive(items, 0);

            return tableRows;

            void AddRowsRecursive(IEnumerable<(string[] parts, string file, SourceFile sourceFile)> items, int level)
            {
                var groups = items
                    .GroupBy(x => x.parts.ElementAtOrDefault(level))
                    .Where(g => g.Key != null);

                foreach (var group in groups)
                {
                    if (group.Count() == 1)
                    {
                        var firstItem = group.First();
                        tableRows.Add(new TableRow
                        {
                            Level = level,
                            Name = group.Key,
                            File = firstItem.file,
                            SourceFiles = group.Select(x => x.sourceFile).ToArray()
                        });
                    }
                    else
                    {
                        tableRows.Add(new TableRow
                        {
                            Level = level,
                            Name = group.Key,
                            File = null,
                            SourceFiles = group.Select(x => x.sourceFile).ToArray()
                        });

                        AddRowsRecursive(group, level + 1);
                    }
                }
            }
        }

        private class TableRow
        {
            public int Level { get; set; }
            public string Name { get; set; }
            public string File { get; set; }
            public SourceFile[] SourceFiles { get; set; }
            public string Class { get; set; }
        }
    }
}
