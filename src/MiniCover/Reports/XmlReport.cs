using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using MiniCover.Model;

namespace MiniCover.Reports
{
    public static class XmlReport
    {
        public static void Execute(InstrumentationResult result, string output, float threshold)
        {

            var hitLines = File.Exists(result.HitsFile)
                ? File.ReadAllLines(result.HitsFile)
                : new string[0];

            var hits = Hits.Parse(hitLines);

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
                    var hitInstructions = file.Value.Instructions.Where(h => hits.Contains(h.Id)).ToArray();

                    return file.Value.Instructions
                        .GroupBy(instruction => new { instruction.Class, instruction.Method, instruction.MethodFullName })
                        .Select(instruction =>
                    {
                        var method = new XElement(
                            XName.Get("method"),
                            new XAttribute(XName.Get("name"), instruction.Key.Method),
                            new XAttribute(XName.Get("class"), instruction.Key.Class),
                            new XAttribute(XName.Get("excluded"), "false"),
                            new XAttribute(XName.Get("instrumented"), "true"),
                            new XAttribute(XName.Get("fullname"), instruction.Key.MethodFullName)
                        );

                        var methodPoints = instruction.Select(methodPoint =>
                        {
                            var methodHits = hits.ForMethod(methodPoint.Id).ToArray();

                            var point = new XElement(
                                XName.Get("seqpnt"),
                                new XAttribute(XName.Get("visitcount"), methodHits.Length),
                                new XAttribute(XName.Get("line"), methodPoint.StartLine),
                                new XAttribute(XName.Get("column"), methodPoint.StartColumn),
                                new XAttribute(XName.Get("endline"), methodPoint.EndLine),
                                new XAttribute(XName.Get("endcolumn"), methodPoint.EndColumn),
                                new XAttribute(XName.Get("excluded"), "false"),
                                new XAttribute(XName.Get("document"), Path.Combine(result.SourcePath, file.Key))
                                );

                            var tests = methodHits.Select(h => new XElement(XName.Get("tests"),
                                new XAttribute(XName.Get("assembly"), h.AssemblyName),
                                new XAttribute(XName.Get("class"), h.ClassName),
                                new XAttribute(XName.Get("method"), h.MethodName),
                                new XAttribute(XName.Get("assemblyLocation"), h.AssemblyLocation)
                                ));

                            point.Add(tests);

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

            File.WriteAllText(output, document.ToString());
        }
    }
}