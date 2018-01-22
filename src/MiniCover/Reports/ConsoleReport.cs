using MiniCover.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MiniCover.Reports
{
    public class ConsoleReport
    {
        private int _fileColumnLength;

        public virtual int Execute(InstrumentationResult result, float threshold)
        {
            var hits = File.Exists(result.HitsFile)
                   ? File.ReadAllLines(result.HitsFile).Select(h => int.Parse(h)).ToArray()
                   : new int[0];

            var files = result.Assemblies
                .SelectMany(assembly => assembly.Value.Files)
                .ToDictionary(
                    x => x.Key,
                    x => x.Value
                );

            _fileColumnLength = files.Keys.Select(s => s.Length).Concat(new[] { 10 }).Max();

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

            WriteFooter(totalLines, totalCoveredLines, totalCoveragePercentage, threshold,totalsColor);

            return isHigherThanThreshold ? 0 : 1;
        }

        protected virtual void WriteHeader()
        {
            WriteHorizontalLine(_fileColumnLength);

            Write("| ");
            Write(Pad("File", _fileColumnLength));
            Write(" | ");
            Write(Pad("Lines", 5, Align.Center));
            Write(" | ");
            Write(Pad("Covered Lines", 13, Align.Center));
            Write(" | ");
            Write(Pad("Percentage", 10, Align.Center));
            Write(" |");
            Write(Environment.NewLine);

            WriteHorizontalLine(_fileColumnLength);
        }

        protected virtual void WriteReport(KeyValuePair<string, SourceFile> kvFile, int lines, int coveredLines, float coveragePercentage, ConsoleColor color)
        {
            Write("| ");
            Write(Pad(kvFile.Key, _fileColumnLength), color);
            Write(" | ");
            Write(Pad(lines.ToString().PadLeft(3), 5, Align.Center), color);
            Write(" | ");
            Write(Pad(coveredLines.ToString().PadLeft(3), 13, Align.Center), color);
            Write(" | ");
            Write(Pad(coveragePercentage.ToString("P").PadLeft(8), 10, Align.Center), color);
            Write(" |");
            Write(Environment.NewLine);
        }

        protected virtual void WriteDetailedReport(InstrumentationResult result, IDictionary<string, SourceFile> files, int[] hits)
        {
        }

        protected virtual void WriteFooter(int lines, int coveredLines, float coveragePercentage, float threshold, ConsoleColor color)
        {
            WriteHorizontalLine(_fileColumnLength);

            Write("| ");
            Write(Pad("All files", _fileColumnLength), color);
            Write(" | ");
            Write(Pad(lines.ToString().PadLeft(3), 5, Align.Center), color);
            Write(" | ");
            Write(Pad(coveredLines.ToString().PadLeft(3), 13, Align.Center), color);
            Write(" | ");
            Write(Pad(coveragePercentage.ToString("P").PadLeft(8), 10, Align.Center), color);
            Write(" |");
            Write(Environment.NewLine);

            WriteHorizontalLine(_fileColumnLength);
        }

        private void WriteHorizontalLine(int fileColumnLength)
        {
            Write("+-");
            Write(new string('-', fileColumnLength));
            Write("-+-");
            Write(new string('-', 5));
            Write("-+-");
            Write(new string('-', 13));
            Write("-+-");
            Write(new string('-', 10));
            Write("-+");
            Write(Environment.NewLine);
        }

        private string Pad(string text, int length, Align alignment = Align.Left)
        {
            switch (alignment)
            {
                case Align.Left:
                    return text.PadRight(length);
                case Align.Center:
                    var left = (int)Math.Ceiling((float)length / 2 + (float)text.Length / 2);
                    return text.PadLeft(left).PadRight(length);
                case Align.Right:
                    return text.PadLeft(length);
                default:
                    throw new InvalidOperationException();
            }
        }

        private void Write(string text)
        {
            Console.Write(text);
        }

        private void Write(string text, ConsoleColor color)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(text);
            Console.ForegroundColor = originalColor;
        }

        private enum Align
        {
            Left,
            Center,
            Right
        }
    }
}
