using System.Collections.Generic;
using System.IO;
using System.Linq;
using MiniCover.Model;

namespace MiniCover.Reports.Helpers
{
    public static class SummaryHelpers
    {
        public static List<SummaryRow> GetSummaryGrid(
            SourceFile[] sourceFiles,
            HitsInfo hitsInfo,
            float threshold)
        {
            var rows = new List<SummaryRow>
            {
                new SummaryRow
                {
                    Level = 0,
                    Name = "All Files",
                    FullName = "All Files",
                    SourceFiles = sourceFiles,
                    Root = true,
                    Summary = CalculateFilesSummary(sourceFiles, hitsInfo, threshold)
                }
            };

            var allItems = sourceFiles
                .Select(file => (
                    file.Path.Split(new[] { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar }),
                    file
                )).ToArray();

            AddRowsRecursive(allItems, 1, string.Empty);

            return rows;

            void AddRowsRecursive(
                IEnumerable<(string[] parts, SourceFile sourceFile)> items,
                int level,
                string baseName)
            {
                var groups = items
                    .GroupBy(x => x.parts.ElementAtOrDefault(level - 1))
                    .Where(g => g.Key != null);

                foreach (var group in groups)
                {
                    var groupSourceFiles = group.Select(x => x.sourceFile).ToArray();

                    var name = groupSourceFiles.Length == 1
                        ? string.Join(Path.DirectorySeparatorChar, group.First().parts.Skip(level - 1))
                        : group.Key;

                    var row = new SummaryRow
                    {
                        Level = level,
                        Name = name,
                        FullName = $"{baseName}{name}",
                        Folder = group.Count() > 1,
                        File = group.Count() == 1,
                        SourceFiles = groupSourceFiles,
                        Summary = CalculateFilesSummary(
                            groupSourceFiles,
                            hitsInfo,
                            threshold)
                    };
                    rows.Add(row);

                    if (group.Count() > 1)
                    {
                        AddRowsRecursive(group, level + 1, $"{baseName}{Path.DirectorySeparatorChar}{group.Key}");
                    }
                }
            }
        }

        public static Summary CalculateFilesSummary(
            IEnumerable<SourceFile> sourceFiles,
            HitsInfo hitsInfo,
            float threshold)
        {
            var summary = new Summary();

            summary.Statements = sourceFiles.Sum(x =>
                x.Sequences
                    .Count());

            summary.CoveredStatements = sourceFiles.Sum(x =>
                x.Sequences
                    .Where(h => hitsInfo.WasHit(h.HitId))
                    .Count());

            summary.Lines = sourceFiles.Sum(x =>
                x.Sequences
                    .SelectMany(s => s.GetLines())
                    .Distinct()
                    .Count());

            summary.CoveredLines = sourceFiles.Sum(x =>
                x.Sequences
                    .Where(h => hitsInfo.WasHit(h.HitId))
                    .SelectMany(s => s.GetLines())
                    .Distinct()
                    .Count());

            summary.Branches = sourceFiles.Sum(x =>
                x.Sequences
                    .SelectMany(s => s.Conditions)
                    .SelectMany(c => c.Branches)
                    .Count());

            summary.CoveredBranches = sourceFiles.Sum(x =>
                x.Sequences
                    .SelectMany(s => s.Conditions)
                    .SelectMany(c => c.Branches)
                    .Where(b => hitsInfo.WasHit(b.HitId))
                    .Count());

            summary.StatementsPercentage = summary.Statements == 0 ? 1 : (float)summary.CoveredStatements / summary.Statements;
            summary.StatementsCoveragePass = summary.StatementsPercentage >= threshold;
            summary.LinesPercentage = summary.Lines == 0 ? 1 : (float)summary.CoveredLines / summary.Lines;
            summary.LinesCoveragePass = summary.LinesPercentage >= threshold;
            summary.BranchesPercentage = summary.Branches == 0 ? 1 : (float)summary.CoveredBranches / summary.Branches;
            summary.BranchesCoveragePass = summary.BranchesPercentage >= threshold;

            return summary;
        }
    }
}
