using System;
using System.Linq;
using MiniCover.Model;
using MiniCover.Reports.Helpers;

namespace MiniCover.Reports
{
    public class ConsoleReport : IConsoleReport
    {
        public int Execute(InstrumentationResult result, float threshold)
        {
            var hitsInfo = HitsInfo.TryReadFromDirectory(result.HitsPath);

            var files = result.GetSourceFiles();

            var summary = SummaryFactory.CalculateFilesSummary(files, hitsInfo, threshold);

            var tableRows = SummaryFactory.GetSummaryGrid(files, hitsInfo, threshold);

            var consoleTable = new ConsoleTable
            {
                Header = CreateHeader(),
                Body = tableRows.Where(r => !r.Root).Select(f => CreateRow(f)).ToArray(),
                Footer = CreateFooter(summary)
            };

            consoleTable.WriteTable();

            return summary.LinesCoveragePass ? 0 : 1;
        }

        private ConsoleRow CreateHeader()
        {
            var row = new ConsoleRow();
            row.Cells.Add(new ConsoleCell("File"));
            row.Cells.Add(new ConsoleCell("Lines", TextAlign.Center));
            row.Cells.Add(new ConsoleCell("% Lines", TextAlign.Center));
            row.Cells.Add(new ConsoleCell("Stmts", TextAlign.Center));
            row.Cells.Add(new ConsoleCell("% Stmts", TextAlign.Center));
            row.Cells.Add(new ConsoleCell("Branches", TextAlign.Center));
            row.Cells.Add(new ConsoleCell("% Branches", TextAlign.Center));
            return row;
        }

        private ConsoleRow CreateRow(SummaryRow r)
        {
            var summary = r.Summary;

            var linesColor = summary.LinesCoveragePass ? ConsoleColor.Green : ConsoleColor.Red;
            var statementsColor = summary.StatementsCoveragePass ? ConsoleColor.Green : ConsoleColor.Red;
            var branchesColor = summary.BranchesCoveragePass ? ConsoleColor.Green : ConsoleColor.Red;

            var row = new ConsoleRow();
            var indentation = Math.Max(r.Level - 1, 0) * 2;
            row.Cells.Add(new ConsoleCell($"{new string(' ', indentation)}{r.Name}"));
            row.Cells.Add(new ConsoleCell($"{summary.CoveredLines}/{summary.Lines}", TextAlign.Right, linesColor));
            row.Cells.Add(new ConsoleCell($"{summary.LinesPercentage:P}", TextAlign.Right, linesColor));
            row.Cells.Add(new ConsoleCell($"{summary.CoveredStatements}/{summary.Statements}", TextAlign.Right, linesColor));
            row.Cells.Add(new ConsoleCell($"{summary.StatementsPercentage:P}", TextAlign.Right, statementsColor));
            row.Cells.Add(new ConsoleCell($"{summary.CoveredBranches}/{summary.Branches}", TextAlign.Right, linesColor));
            row.Cells.Add(new ConsoleCell($"{summary.BranchesPercentage:P}", TextAlign.Right, branchesColor));
            return row;
        }

        private ConsoleRow CreateFooter(Summary summary)
        {
            var linesColor = summary.LinesCoveragePass ? ConsoleColor.Green : ConsoleColor.Red;
            var statementsColor = summary.StatementsCoveragePass ? ConsoleColor.Green : ConsoleColor.Red;
            var branchesColor = summary.BranchesCoveragePass ? ConsoleColor.Green : ConsoleColor.Red;

            var row = new ConsoleRow();
            row.Cells.Add(new ConsoleCell("All files"));
            row.Cells.Add(new ConsoleCell($"{summary.CoveredLines}/{summary.Lines}", TextAlign.Right, linesColor));
            row.Cells.Add(new ConsoleCell($"{summary.LinesPercentage:P}", TextAlign.Right, linesColor));
            row.Cells.Add(new ConsoleCell($"{summary.CoveredStatements}/{summary.Statements}", TextAlign.Right, linesColor));
            row.Cells.Add(new ConsoleCell($"{summary.StatementsPercentage:P}", TextAlign.Right, statementsColor));
            row.Cells.Add(new ConsoleCell($"{summary.CoveredBranches}/{summary.Branches}", TextAlign.Right, linesColor));
            row.Cells.Add(new ConsoleCell($"{summary.BranchesPercentage:P}", TextAlign.Right, branchesColor));
            return row;
        }
    }
}
