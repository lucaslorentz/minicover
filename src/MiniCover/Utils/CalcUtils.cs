using System;
using MiniCover.Model;
using System.Linq;
using System.Xml.Linq;

namespace MiniCover.Utils
{
    public static class CalcUtils
    {
        public static int IsHigherThanThreshold(InstrumentationResult result, float threshold) {
            
            var hits = Hits.TryReadFromFile(result.HitsFile);
            var files = result.GetSourceFiles();
            
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
            }

            var totalCoveragePercentage = (float)totalCoveredLines / totalLines;
            var isHigherThanThreshold = totalCoveragePercentage >= threshold;

            return isHigherThanThreshold ? 0 : 1;
        }
    }
}
