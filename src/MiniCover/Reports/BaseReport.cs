using MiniCover.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniCover.Reports
{
    public abstract class BaseReport
    {
        public virtual int Execute(InstrumentationResult result, float threshold)
        {
            var hits = HitsInfo.TryReadFromDirectory(result.HitsPath);

            var files = result.GetSourceFiles();

            SetFileColumnLength(files.Keys.Select(s => s.Length).Concat(new[] { 10 }).Max());

            WriteHeader();

            var totalLines = 0;
            var totalCoveredLines = 0;

            foreach (var kvFile in files)
            {
                var lines = kvFile.Value.Instructions
                    .SelectMany(i => i.GetLines())
                    .Distinct()
                    .Count();

                var coveredLines = kvFile.Value.Instructions
                    .Where(h => hits.IsInstructionHit(h.Id))
                    .SelectMany(i => i.GetLines())
                    .Distinct()
                    .Count();

                totalLines += lines;
                totalCoveredLines += coveredLines;

                var coveragePercentage = (float)coveredLines / lines;
                var fileColor = coveragePercentage >= threshold ? ConsoleColor.Green : ConsoleColor.Red;

                WriteReport(kvFile, lines, coveredLines, coveragePercentage, fileColor);
            }

            WriteDetailedReport(result, files, hits);

            var totalCoveragePercentage = (float)totalCoveredLines / totalLines;
            var isHigherThanThreshold = totalCoveragePercentage >= threshold;
            var totalsColor = isHigherThanThreshold ? ConsoleColor.Green : ConsoleColor.Red;

            WriteFooter(totalLines, totalCoveredLines, totalCoveragePercentage, threshold, totalsColor);

            return isHigherThanThreshold ? 0 : 1;
        }

        protected abstract void SetFileColumnLength(int fileColumnsLength);

        protected abstract void WriteHeader();

        protected abstract void WriteReport(KeyValuePair<string, SourceFile> kvFile, int lines, int coveredLines, float coveragePercentage, ConsoleColor color);

        protected abstract void WriteDetailedReport(InstrumentationResult result, IDictionary<string, SourceFile> files, HitsInfo hits);

        protected abstract void WriteFooter(int lines, int coveredLines, float coveragePercentage, float threshold, ConsoleColor color);
    }
}
