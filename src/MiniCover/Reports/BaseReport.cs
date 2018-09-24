using MiniCover.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MiniCover.Reports
{

    class CoverLine
    {
        public int InstructionId = -1;
        public int InstructionLineCounts = int.MaxValue;
        public int Hits;
    }

    public abstract class BaseReport
    {
        private void WriteWarning(string Warning)
        {
            ConsoleColor original = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("Warning: ");
            Console.WriteLine(Warning);
            Console.ForegroundColor = original;
        }

        public virtual int Execute(InstrumentationResult result, float threshold)
        {
            var hits = Hits.TryReadFromFile(result.HitsFile);

            var files = result.GetSourceFiles();

            SetFileColumnLength(files.Keys.Select(s => s.Length).Concat(new[] { 10 }).Max());

            WriteHeader();

            var totalLines = 0;
            var totalCoveredLines = 0;

            foreach (var kvFile in files)
            {
                /*
                var lines = kvFile.Value.Instructions
                    .SelectMany(i => i.GetLines())
                    .Distinct()
                    .Count();

                var coveredLines = kvFile.Value.Instructions
                    .Where(h => hits.IsInstructionHit(h.Id))
                    .SelectMany(i => i.GetLines())
                    .Distinct()
                    .Count();

                
                */

                var SourceFile  = System.IO.Path.Combine(result.SourcePath, kvFile.Key);
                var SourceLines = File.ReadAllLines(SourceFile);
                 
                CoverLine[] covlines = new CoverLine[SourceLines.Length];

                foreach (var instruction in kvFile.Value.Instructions)
                {
                    int InstructionLineCounts = instruction.EndLine - instruction.StartLine;
                    for (int l = instruction.StartLine; l <= instruction.EndLine; l++)
                    {

                        CoverLine line = covlines[(l - 1)];

                        if (null == line)
                        {
                            covlines[(l - 1)] = line = new CoverLine();

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
                
                int coveredLines = 0;
                int lines = 0;
                foreach (var l in covlines)
                {
                    
                    string hit = (l == null || 0 > l.Hits) ? "null" : l.Hits.ToString();

                    if ("null" != hit && 0 < l.Hits)
                        coveredLines++;

                    if (null != l)
                        lines++;


                }

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

        protected abstract void WriteDetailedReport(InstrumentationResult result, IDictionary<string, SourceFile> files, Hits hits);

        protected abstract void WriteFooter(int lines, int coveredLines, float coveragePercentage, float threshold, ConsoleColor color);
    }
}
