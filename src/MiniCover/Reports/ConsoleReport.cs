using MiniCover.Model;
using System;
using System.Collections.Generic;

namespace MiniCover.Reports
{
    public class ConsoleReport : BaseReport
    {
        private int _fileColumnLength;

        protected override void SetFileColumnLength(int fileColumnsLength)
        {
            _fileColumnLength = fileColumnsLength;
        }

        protected override void WriteHeader()
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

        protected override void WriteReport(KeyValuePair<string, SourceFile> kvFile, int lines, int coveredLines, float coveragePercentage, ConsoleColor color)
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

        protected override void WriteDetailedReport(InstrumentationResult result, IDictionary<string, SourceFile> files, HitsInfo hits)
        {
        }

        protected override void WriteFooter(int lines, int coveredLines, float coveragePercentage, float threshold, ConsoleColor color)
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
