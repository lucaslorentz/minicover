using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using MiniCover.Model;
using MiniCover.Reports.Helpers;
using MiniCover.Utils;

namespace MiniCover.Reports.Html
{
    public class HtmlSourceFileReport
    {
        public void Generate(
            InstrumentationResult result,
            SourceFile sourceFile,
            HitsInfo hitsInfo,
            float threshold,
            string outputFile)
        {
            var lines = File.ReadAllLines(Path.Combine(result.SourcePath, sourceFile.Path));

            Directory.CreateDirectory(Path.GetDirectoryName(outputFile));

            var summary = SummaryFactory.CalculateFilesSummary(new[] { sourceFile }, hitsInfo, threshold);

            var lineCoverageClass = summary.LinesCoveragePass ? "green" : "red";
            var statementsCoverageClass = summary.StatementsCoveragePass ? "green" : "red";
            var branchCoverageClass = summary.BranchesCoveragePass ? "green" : "red";

            using (var htmlWriter = (TextWriter)File.CreateText(outputFile))
            {
                htmlWriter.WriteLine("<html>");
                htmlWriter.WriteLine("<style>");
                htmlWriter.WriteLine(ResourceUtils.GetContent("MiniCover.Reports.Html.Shared.css"));
                htmlWriter.WriteLine(ResourceUtils.GetContent("MiniCover.Reports.Html.SourceFile.css"));
                htmlWriter.WriteLine("</style>");
                htmlWriter.WriteLine("<script>");
                htmlWriter.WriteLine(ResourceUtils.GetContent("MiniCover.Reports.Html.Shared.js"));
                htmlWriter.WriteLine("</script>");
                htmlWriter.WriteLine("<body>");

                htmlWriter.WriteLine("<h2>Summary</h2>");
                htmlWriter.WriteLine("<table>");
                htmlWriter.WriteLine($"<tr><th>Generated on</th><td>{DateTime.Now}</td></tr>");
                htmlWriter.WriteLine($"<tr><th>Line Coverage</th><td class=\"{lineCoverageClass}\">{summary.LinesPercentage:P} ({summary.CoveredLines}/{summary.Lines})</td></tr>");
                htmlWriter.WriteLine($"<tr><th>Statements Coverage</th><td class=\"{branchCoverageClass}\">{summary.StatementsPercentage:P} ({summary.CoveredStatements}/{summary.Statements})</td></tr>");
                htmlWriter.WriteLine($"<tr><th>Branch Coverage</th><td class=\"{branchCoverageClass}\">{summary.BranchesPercentage:P} ({summary.CoveredBranches}/{summary.Branches})</td></tr>");
                htmlWriter.WriteLine($"<tr><th>Threshold</th><td>{threshold:P}</td></tr>");
                htmlWriter.WriteLine("</table>");

                htmlWriter.WriteLine("<h2>Code</h2>");
                htmlWriter.WriteLine("<div class=\"legend\">");
                htmlWriter.Write("<label>Legend:</label>");
                htmlWriter.Write("<div class=\"hit\">Covered</div>");
                htmlWriter.Write("<div class=\"partial\">Partially covered</div>");
                htmlWriter.Write("<div class=\"not-hit\">Not covered</div>");
                htmlWriter.WriteLine("</div>");
                htmlWriter.WriteLine("<div class=\"code\">");
                for (var l = 1; l <= lines.Length; l++)
                {
                    var line = lines[l - 1];

                    var instructions = sourceFile.Sequences
                        .Where(i => i.GetLines().Contains(l))
                        .ToArray();

                    var lineHitCount = instructions.Sum(a => hitsInfo.GetHitCount(a.HitId));

                    var lineClasses = new List<string> { "line" };

                    if (lineHitCount > 0)
                    {
                        if (instructions.Any(i => !hitsInfo.WasHit(i.HitId)
                            || i.Conditions.SelectMany(x => x.Branches).Any(b => !hitsInfo.WasHit(b.HitId))))
                        {
                            lineClasses.Add("partial");
                        }
                        else
                        {
                            lineClasses.Add("hit");
                        }
                    }
                    else if (instructions.Length > 0)
                    {
                        lineClasses.Add("not-hit");
                    }

                    htmlWriter.Write($"<div class=\"{string.Join(" ", lineClasses)}\">");

                    htmlWriter.Write($"<div class=\"line-number\">{l}</div>");

                    htmlWriter.Write("<div class=\"line-content\">");

                    if (line.Length > 0)
                    {
                        for (var c = 1; c <= line.Length; c++)
                        {
                            var character = line[c - 1].ToString();

                            foreach (var instruction in instructions)
                            {
                                if (instruction.StartLine == l && instruction.StartColumn == c
                                    || instruction.StartLine < l && c == 1)
                                {
                                    var statementIdClass = $"s-{instruction.HitId}";

                                    var statementClasses = new List<string> { "statement", statementIdClass };

                                    if (hitsInfo.WasHit(instruction.HitId))
                                    {
                                        statementClasses.Add("hit");

                                        if (instruction.Conditions.SelectMany(x => x.Branches).Any(b => !hitsInfo.WasHit(b.HitId)))
                                        {
                                            statementClasses.Add("partial");
                                        }
                                    }
                                    else
                                    {
                                        statementClasses.Add("not-hit");
                                    }

                                    htmlWriter.Write($"<div data-hover-target=\".{statementIdClass}\" data-activate-target=\".{statementIdClass}\" class=\"{string.Join(" ", statementClasses)}\">");

                                    if (instruction.EndLine == l)
                                    {
                                        var hitCount = hitsInfo.GetHitCount(instruction.HitId);

                                        var contexts = hitsInfo.GetHitContexts(instruction.HitId)
                                            .Distinct()
                                            .ToArray();

                                        htmlWriter.Write($"<div class=\"statement-info {statementIdClass}\">");
                                        htmlWriter.Write($"<div>Id: {instruction.HitId}</div>");
                                        htmlWriter.Write($"<div>Hits: {hitCount}</div>");
                                        if (instruction.Conditions.Length > 0)
                                        {
                                            var conditionIndex = 0;
                                            foreach (var condition in instruction.Conditions)
                                            {
                                                htmlWriter.Write($"<div>Condition {++conditionIndex}:");
                                                htmlWriter.Write("<ul>");
                                                var branchIndex = 0;
                                                foreach (var branch in condition.Branches)
                                                {
                                                    var branchHitCount = hitsInfo.GetHitCount(branch.HitId);
                                                    htmlWriter.Write($"<li>Branch {++branchIndex}: {FormatHits(branchHitCount)}</li>");
                                                }
                                                htmlWriter.Write("</ul>");
                                                htmlWriter.Write("</div>");
                                            }
                                        }
                                        if (contexts.Length > 0)
                                        {
                                            htmlWriter.Write("<div>Contexts:");
                                            htmlWriter.Write("<ul>");
                                            foreach (var context in contexts)
                                            {
                                                var contextHitCount = context.GetHitCount(instruction.HitId);
                                                var description = $"{context.ClassName}.{context.MethodName}";
                                                htmlWriter.Write($"<li>{WebUtility.HtmlEncode(description)}: {FormatHits(contextHitCount)}</li>");
                                            }
                                            htmlWriter.Write("</ul></div>");
                                        }
                                        htmlWriter.Write("</div>");
                                    }
                                }
                            }

                            htmlWriter.Write(WebUtility.HtmlEncode(character));

                            foreach (var instruction in instructions)
                            {
                                if (instruction.EndLine == l && instruction.EndColumn == c + 1
                                    || instruction.EndLine > l && c == line.Length)
                                {
                                    htmlWriter.Write("</div>");
                                }
                            }
                        }
                    }
                    else
                    {
                        htmlWriter.WriteLine("&nbsp;");
                    }

                    htmlWriter.Write("</div>");
                    htmlWriter.WriteLine("</div>");
                }

                htmlWriter.WriteLine("</div>");
                htmlWriter.WriteLine("</body>");
                htmlWriter.WriteLine("</html>");
            }
        }

        private string FormatHits(int count)
        {
            if (count == 1)
                return $"{count} hit";

            return $"{count} hits";
        }
    }
}
