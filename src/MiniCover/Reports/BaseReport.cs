using MiniCover.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MiniCover.Reports
{
    public abstract class BaseReport
    {
        public virtual int Execute(InstrumentationResult result, float threshold)
        {
            var hits = File.Exists(result.HitsFile)
                               ? File.ReadAllLines(result.HitsFile).Select(h => int.Parse(h)).Distinct().ToHashSet()
                               : new HashSet<int>();

            var files = result.Assemblies
                .SelectMany(assembly => assembly.Value.Files)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value
                );

            SetFileColumnLength(files.Keys.Select(s => s.Length).Concat(new[] { 10 }).Max());

            WriteHeader();

            var totalLines = 0;
            var totalCoveredLines = 0;

            foreach (var kvFile in files)
            {
                var lines = kvFile.Value.Instructions
                    .SelectMany(i => Enumerable.Range(i.StartLine, i.EndLine - i.StartLine + 1))
                    .Distinct()
                    .Count();

                var hitInstructions = kvFile.Value.Instructions.Where(h => hits.Contains(h.Id)).ToArray();
                var coveredLines = hitInstructions
                    .SelectMany(i => Enumerable.Range(i.StartLine, i.EndLine - i.StartLine + 1))
                    .Distinct()
                    .Count();

                totalLines += lines;
                totalCoveredLines += coveredLines;

                var coveragePercentage = (float)coveredLines / lines;
                var fileColor = coveragePercentage >= threshold ? ConsoleColor.Green : ConsoleColor.Red;

                WriteReport(kvFile, lines, coveredLines, coveragePercentage, fileColor);

                WriteDetailedReport(result, files, hits);
            }

            var totalCoveragePercentage = (float)totalCoveredLines / totalLines;
            var isHigherThanThreshold = totalCoveragePercentage >= threshold;
            var totalsColor = isHigherThanThreshold ? ConsoleColor.Green : ConsoleColor.Red;

            WriteFooter(totalLines, totalCoveredLines, totalCoveragePercentage, threshold, totalsColor);

            return isHigherThanThreshold ? 0 : 1;
        }

        protected abstract void SetFileColumnLength(int fileColumnsLength);

        protected abstract void WriteHeader();

        protected abstract void WriteReport(KeyValuePair<string, SourceFile> kvFile, int lines, int coveredLines, float coveragePercentage, ConsoleColor color);

        protected abstract void WriteDetailedReport(InstrumentationResult result, IDictionary<string, SourceFile> files, HashSet<int> hits);

        protected abstract void WriteFooter(int lines, int coveredLines, float coveragePercentage, float threshold, ConsoleColor color);
    }
}
