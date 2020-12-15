using System;
using System.Collections.Generic;
using System.Linq;

namespace MiniCover.Reports.Helpers
{
    public class ConsoleTable
    {
        public ConsoleRow Header { get; set; }
        public ConsoleRow[] Body { get; set; }
        public ConsoleRow Footer { get; set; }

        public void WriteTable()
        {
            var allRows = (Header != null ? new[] { Header } : Enumerable.Empty<ConsoleRow>())
                .Concat(Body)
                .Concat(Footer != null ? new[] { Footer } : Enumerable.Empty<ConsoleRow>())
                .ToArray();

            int[] columnsLengths = ComputeCellSizes(allRows);

            WriteHorizontalLine(columnsLengths, BoxPart.Bottom);
            if (Header != null)
            {
                WriteRow(columnsLengths, Header);
                WriteHorizontalLine(columnsLengths, BoxPart.Vertical);
            }
            foreach (var bodyRow in Body)
            {
                WriteRow(columnsLengths, bodyRow);
            }
            if (Footer != null)
            {
                WriteHorizontalLine(columnsLengths, BoxPart.Vertical);
                WriteRow(columnsLengths, Footer);
            }
            WriteHorizontalLine(columnsLengths, BoxPart.Top);
        }

        private void WriteRow(int[] columnsLengths, ConsoleRow row)
        {
            WriteBox(BoxPart.Vertical);
            for (var c = 0; c < columnsLengths.Length; c++)
            {
                var cell = row.Cells.Count > c
                    ? row.Cells[c]
                    : ConsoleCell.Empty;

                Write(" ");
                Write(Pad(cell?.Text ?? "", columnsLengths[c], cell.Align), cell.Color);
                Write(" ");
                WriteBox(BoxPart.Vertical);
            }
            Write(Environment.NewLine);
        }

        private void WriteHorizontalLine(int[] columnsLengths, BoxPart connections)
        {
            for (var c = 0; c < columnsLengths.Length; c++)
            {
                if (c == 0)
                    WriteBox(BoxPart.Right | connections);

                WriteBox(BoxPart.Horizontal);
                WriteBox(BoxPart.Left | BoxPart.Right, columnsLengths[c]);
                WriteBox(BoxPart.Horizontal);

                if (c == columnsLengths.Length - 1)
                    WriteBox(BoxPart.Left | connections);
                else
                    WriteBox(BoxPart.Left | BoxPart.Right | connections);
            }

            Write(Environment.NewLine);
        }

        private int[] ComputeCellSizes(IEnumerable<ConsoleRow> allRows)
        {
            var numberOfColumns = allRows.Max(r => r.Cells.Count);

            var cellsSizes = new int[numberOfColumns];

            foreach (var row in allRows)
            {
                for (var c = 0; c < row.Cells.Count; c++)
                {
                    var length = row.Cells[c].Text.Length;
                    if (length > cellsSizes[c])
                        cellsSizes[c] = length;
                }
            }

            return cellsSizes;
        }

        private string Pad(string text, int length, TextAlign alignment = TextAlign.Left)
        {
            switch (alignment)
            {
                case TextAlign.Left:
                    return text.PadRight(length);
                case TextAlign.Center:
                    var left = (int)Math.Ceiling((float)length / 2 + (float)text.Length / 2);
                    return text.PadLeft(left).PadRight(length);
                case TextAlign.Right:
                    return text.PadLeft(length);
                default:
                    throw new InvalidOperationException();
            }
        }

        private void WriteBox(BoxPart parts, int repeat = 1)
        {
            System.Console.Write(new string(parts.ToChar(), repeat));
        }

        private void Write(string text, ConsoleColor? color = null)
        {
            var originalColor = System.Console.ForegroundColor;
            System.Console.ForegroundColor = color ?? originalColor;
            System.Console.Write(text);
            System.Console.ForegroundColor = originalColor;
        }
    }

    public class ConsoleRow
    {
        public List<ConsoleCell> Cells { get; set; } = new List<ConsoleCell>();
    }

    public class ConsoleCell
    {
        public ConsoleCell(
            string text,
            TextAlign align = TextAlign.Left,
            ConsoleColor? color = null)
        {
            Text = text;
            Align = align;
            Color = color;
        }

        public string Text { get; set; }
        public TextAlign Align { get; set; }
        public ConsoleColor? Color { get; set; }

        public static ConsoleCell Empty { get; } = new ConsoleCell("");
    }

    public enum TextAlign
    {
        Left,
        Center,
        Right
    };
}
