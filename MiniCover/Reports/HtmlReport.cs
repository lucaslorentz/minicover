using MiniCover.Instrumentation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace MiniCover.Reports
{
    public class HtmlReport
    {
        public static void Execute(InstrumentationResult result, float threshold)
        {
            var hits = File.Exists(result.HitsFile)
                   ? File.ReadAllLines(result.HitsFile).Select(h => int.Parse(h)).ToArray()
                   : new int[0];

            foreach (var kvFile in result.Files)
            {
                var lines = File.ReadAllLines(Path.Combine(result.SourcePath, kvFile.Key));

                var fileName = Path.Combine("coverage-html", kvFile.Key + ".html");

                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

                using (var htmlWriter = (TextWriter)File.CreateText(fileName))
                {
                    htmlWriter.WriteLine("<html>");
                    htmlWriter.WriteLine("<body>");

                    var instrumentedLineNumbers = kvFile.Value.Instructions
                        .SelectMany(i => Enumerable.Range(i.StartLine, i.EndLine - i.StartLine + 1))
                        .Distinct()
                        .ToArray();

                    var hitInstructions = kvFile.Value.Instructions.Where(h => hits.Contains(h.Id)).ToArray();
                    var coveredLineNumbers = hitInstructions
                        .SelectMany(i => Enumerable.Range(i.StartLine, i.EndLine - i.StartLine + 1))
                        .Distinct()
                        .ToArray();

                    var l = 0;
                    foreach (var line in lines)
                    {
                        l++;
                        var style = "white-space: pre;";
                        if (instrumentedLineNumbers.Contains(l))
                        {
                            if (coveredLineNumbers.Contains(l))
                            {
                                style += "background-color: #D2EACE;";
                            }
                            else
                            {
                                style += "background-color: #EACECC;";
                            }
                        }
                        else
                        {
                            style += "background-color: #EEF4ED;";
                        }

                        htmlWriter.WriteLine($"<div style=\"{style}\">{WebUtility.HtmlEncode(line)}</div>");
                    }

                    htmlWriter.WriteLine("</body>");
                    htmlWriter.WriteLine("</html>");
                }
            }
        }
    }
}
