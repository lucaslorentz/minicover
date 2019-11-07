using MiniCover.Model;
using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MiniCover.Reports
{
    public static class XmlReport
    {
        public static void Execute(InstrumentationResult result, FileInfo output, float threshold)
        {
            var hits = HitsInfo.TryReadFromDirectory(result.HitsPath);

            var data = new XProcessingInstruction("xml-stylesheet", "type='text/xsl' href='coverage.xsl'");

            var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), data);

            var coverageElement = new XElement(
                XName.Get("coverage"),
                new XAttribute(XName.Get("profilerVersion"), 0),
                new XAttribute(XName.Get("driverVersion"), 0),
                new XAttribute(XName.Get("startTime"), DateTime.MaxValue.ToString("o")),
                new XAttribute(XName.Get("measureTime"), DateTime.MinValue.ToString("o"))
            );

            var modules = result.Assemblies.Select(assembly =>
            {
                var module = new XElement(
                    XName.Get("module"),
                    new XAttribute(XName.Get("moduleId"), Guid.NewGuid().ToString()),
                    new XAttribute(XName.Get("name"), assembly.Name),
                    new XAttribute(XName.Get("assembly"), assembly.Name),
                    new XAttribute(XName.Get("assemblyIdentity"), assembly.Name)
                );

                var methods = assembly.SourceFiles.Select(file =>
                {
                    return file.Value.Instructions
                        .GroupBy(instruction => instruction.Method)
                        .Select(instruction =>
                    {
                        var method = new XElement(
                            XName.Get("method"),
                            new XAttribute(XName.Get("name"), instruction.Key.Name),
                            new XAttribute(XName.Get("class"), instruction.Key.Class),
                            new XAttribute(XName.Get("excluded"), "false"),
                            new XAttribute(XName.Get("instrumented"), "true"),
                            new XAttribute(XName.Get("fullname"), instruction.Key.FullName)
                        );

                        var methodPoints = instruction.Select(methodPoint =>
                        {
                            var counter = hits.GetInstructionHitCount(methodPoint.Id);

                            var point = new XElement(
                                XName.Get("seqpnt"),
                                new XAttribute(XName.Get("visitcount"), counter),
                                new XAttribute(XName.Get("line"), methodPoint.StartLine),
                                new XAttribute(XName.Get("column"), methodPoint.StartColumn),
                                new XAttribute(XName.Get("endline"), methodPoint.EndLine),
                                new XAttribute(XName.Get("endcolumn"), methodPoint.EndColumn),
                                new XAttribute(XName.Get("excluded"), "false"),
                                new XAttribute(XName.Get("document"), Path.Combine(result.SourcePath, file.Key))
                                );

                            return point;
                        });

                        method.Add(methodPoints);

                        return method;
                    });
                });

                module.Add(methods);

                return module;
            });

            coverageElement.Add(modules);

            document.Add(coverageElement);

            output.Directory.Create();
            File.WriteAllText(output.FullName, document.ToString());
        }
    }
}