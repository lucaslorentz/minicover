using System.Linq;
using MiniCover.Model;

namespace MiniCover.Utils
{
    public static class CalcUtils
    {
        public static int IsHigherThanThreshold(InstrumentationResult result, float threshold) {
            
            var hits = HitsInfo.TryReadFromDirectory(result.HitsPath);
            var files = result.GetSourceFiles();
            
            var totalLines = 0;
            var totalCoveredLines = 0;
            
            foreach (var file in files)
            {
                var lines = file.Sequences
                    .SelectMany(i => i.GetLines())
                    .Distinct()
                    .Count();

                var coveredLines = file.Sequences
                    .Where(h => hits.WasHit(h.HitId))
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
