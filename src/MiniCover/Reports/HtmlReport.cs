using MiniCover.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

namespace MiniCover.Reports
{
    public class HtmlReport : BaseReport
    {
        private const string BgColorGreen = "background-color: #D2EACE;";
        private const string BgColorRed = "background-color: #EACECC;";
        private const string BgColorBlue = "background-color: #EEF4ED;";
        private readonly string _output;
        private readonly StringBuilder _htmlReport;

        public HtmlReport(string output)
        {
            _output = output;
            _htmlReport = new StringBuilder();
        }

        protected override void SetFileColumnLength(int fileColumnsLength)
        {
        }

        protected override void WriteHeader()
        {
        }

        protected override void WriteReport(KeyValuePair<string, SourceFile> kvFile, int lines, int coveredLines, float coveragePercentage, ConsoleColor color)
        {
            _htmlReport.AppendLine("<tr>");
            _htmlReport.AppendLine($"<td><a href=\"{kvFile.Key}.html\">{kvFile.Key}</a></td>");
            _htmlReport.AppendLine($"<td>{lines}</td>");
            _htmlReport.AppendLine($"<td>{coveredLines}</td>");
            _htmlReport.AppendLine($"<td style=\"{GetBgColor(color)}\">{coveragePercentage:P}</td>");
            _htmlReport.AppendLine("</tr>");
        }

        protected override void WriteDetailedReport(InstrumentationResult result, IDictionary<string, SourceFile> files, HashSet<int> hits)
        {
            foreach (var kvFile in files)
            {
                var lines = File.ReadAllLines(Path.Combine(result.SourcePath, kvFile.Key));

                var fileName = Path.Combine(_output, kvFile.Key + ".html");

                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                var fileBuilder = new StringBuilder();
                fileBuilder.Append("<html>");
                fileBuilder.Append("<body style=\"font-family: monospace;\">");

                var uncoveredLineNumbers = new HashSet<int>();
                var coveredLineNumbers = new HashSet<int>();
                foreach (var i in kvFile.Value.Instructions)
                {
                    if (hits.Contains(i.Id))
                    {
                        coveredLineNumbers.UnionWith(i.GetLines());
                    }
                    else
                    {
                        uncoveredLineNumbers.UnionWith(i.GetLines());
                    }
                }

                var l = 0;
                foreach (var line in lines)
                {
                    l++;
                    var style = "white-space: pre;";
                    if (coveredLineNumbers.Contains(l))
                    {
                        style += BgColorGreen;
                    }
                    else if (uncoveredLineNumbers.Contains(l))
                    {
                        style += BgColorRed;
                    }
                    else
                    {
                        style += BgColorBlue;
                    }

                    if (!string.IsNullOrEmpty(line))
                    {
                        fileBuilder.Append($"<div style=\"{style}\">{WebUtility.HtmlEncode(line)}</div>");
                    }
                    else
                    {
                        fileBuilder.Append($"<div style=\"{style}\"><br/></div>");
                    }
                }

                fileBuilder.Append("</body>");
                fileBuilder.Append("</html>");
                SaveIndentedHtmlFile(fileBuilder.ToString(), fileName);
            }
        }

        protected override void WriteFooter(int lines, int coveredLines, float coveragePercentage, float threshold, ConsoleColor color)
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append("<html>");
            stringBuilder.Append("<body style=\"font-family: sans-serif;\">");

            // Write summary
            stringBuilder.Append("<h1>Summary</h1>");
            stringBuilder.Append("<table border=\"1\" cellpadding=\"5\">");
            stringBuilder.Append($"<tr><th>Generated on</th><td>{DateTime.Now}</td></tr>");
            stringBuilder.Append($"<tr><th>Lines</th><td>{lines}</td></tr>");
            stringBuilder.Append($"<tr><th>Covered Lines</th><td>{coveredLines}</td></tr>");
            stringBuilder.Append($"<tr><th>Threshold</th><td>{threshold:P}</td></tr>");
            stringBuilder.Append($"<tr><th>Percentage</th><td style=\"{GetBgColor(color)}\">{coveragePercentage:P}</td></tr>");
            stringBuilder.Append("</table>");

            // Write detailed report
            stringBuilder.Append("<h1>Coverage</h1>");
            stringBuilder.Append("<table border=\"1\" cellpadding=\"5\">");
            stringBuilder.Append("<tr>");
            stringBuilder.Append("<th>File</th>");
            stringBuilder.Append("<th>Lines</th>");
            stringBuilder.Append("<th>Covered Lines</th>");
            stringBuilder.Append("<th>Percentage</th>");
            stringBuilder.Append("</tr>");
            stringBuilder.Append(_htmlReport);
            stringBuilder.Append("</table>");
            stringBuilder.Append("</body>");
            stringBuilder.Append("</html>");

            var result = stringBuilder.ToString();
            Directory.CreateDirectory(_output);
            var fileName = Path.Combine(_output, "index.html");
            SaveIndentedHtmlFile(result, fileName);
        }

        private static void SaveIndentedHtmlFile(string result, string fileName)
        {
            using (var stream = File.Create(fileName))
            {
                var document = new XmlDocument();
                document.LoadXml(result);
                document.Save(stream);
            }
        }

        private string GetBgColor(ConsoleColor color)
        {
            switch (color)
            {
                case ConsoleColor.Green:
                    return BgColorGreen;

                case ConsoleColor.Red:
                    return BgColorRed;

                default:
                    throw new ArgumentException($"Invalid color: {color}");
            }
        }
    }
}