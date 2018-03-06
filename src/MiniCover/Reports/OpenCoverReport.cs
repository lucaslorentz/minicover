using MiniCover.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace MiniCover.Reports
{
    public static class OpenCoverReport
    {
        public static void Execute(InstrumentationResult result, string output, float threshold)
        {
            var hits = Hits.TryReadFromFile(result.HitsFile);

            int fileIndex = 0;
            int sequencePointMegaCounter = 0;

            var data = new XProcessingInstruction("xml-stylesheet", "type='text/xsl' href='coverage.xsl'");

            var document = new XDocument(new XDeclaration("1.0", "utf-8", "yes"), data);

            var coverageElement = new XElement(
                XName.Get("CoverageSession")
            );

            var modulesListElement = result.Assemblies.Select(assembly =>
            {
                var moduleElement = new XElement(
                    XName.Get("Module"),
                    new XAttribute(XName.Get("hash"), Guid.NewGuid().ToString())
                );

                var fullNameElement = new XElement(
                    XName.Get("FullName"),
                    new XText(assembly.Name)
                );

                var moduleNameElement = new XElement(
                    XName.Get("ModuleName"),
                    new XText(assembly.Name)
                );

                Dictionary<SourceFile, int> dctSourceFileCount = new Dictionary<SourceFile, int>();

                var filesElement = assembly.SourceFiles.Select(file =>
                {
                    dctSourceFileCount.Add(file.Value, ++fileIndex);
                    var fileElement = new XElement(
                        XName.Get("File"),
                        new XAttribute(XName.Get("uid"), dctSourceFileCount[file.Value]),
                        new XAttribute(XName.Get("fullPath"), Path.Combine(result.SourcePath, file.Key))
                    );

                    return fileElement;
                });

                var classesElement = assembly.SourceFiles.Select(file =>
                {
                    var hitInstructions = file.Value.Instructions.Where(h => hits.IsInstructionHit(h.Id)).ToArray();

                    return file.Value.Instructions
                        .GroupBy(instruction => new { instruction.Class })
                        .Select(classes =>
                    {
                        var classElement = new XElement(
                            XName.Get("Class")
                        );

                        var classfullNameElement = new XElement(
                            XName.Get("FullName"),
                            new XText(classes.Key.Class)
                        );

                        var methodsList = classes
                            .GroupBy(instruction => new { instruction.Method, instruction.MethodFullName })
                            .Select(method =>
                            {
                                var nameElement = new XElement(
                                    XName.Get("Name"),
                                    new XText(method.Key.MethodFullName)
                                );

                                var fileRefElement = new XElement(
                                    XName.Get("FileRef"),
                                    new XAttribute(XName.Get("uid"), dctSourceFileCount[file.Value])
                                );

                                int sequencePointMiniCounter = 0;

                                var sequencePoints = method
                                    .OrderBy(methodPoint => methodPoint.StartLine)
                                    .Select(methodPoint =>
                                {
                                    var hitCount = hitInstructions.Count(hit => hit.Equals(methodPoint));

                                    return new XElement(
                                        XName.Get("SequencePoint"),
                                        new XAttribute(XName.Get("vc"), hitCount),
                                        new XAttribute(XName.Get("uspid"), ++sequencePointMegaCounter),
                                        new XAttribute(XName.Get("ordinal"), ++sequencePointMiniCounter),
                                        new XAttribute(XName.Get("sl"), methodPoint.StartLine),
                                        new XAttribute(XName.Get("sc"), methodPoint.StartColumn),
                                        new XAttribute(XName.Get("el"), methodPoint.EndLine),
                                        new XAttribute(XName.Get("ec"), methodPoint.EndColumn)
                                    );
                                });

                                var methodElement = new XElement(
                                    XName.Get("Method"),
                                    new XAttribute(XName.Get("visited"), method.Any(p => hitInstructions.Any(hit => hit == p))),
                                    new XAttribute(XName.Get("isConstructor"), method.Key.Method == ".ctor")
                                );

                                methodElement.Add(nameElement);
                                methodElement.Add(fileRefElement);
                                methodElement.Add(sequencePoints);

                                return methodElement;
                            });

                        classElement.Add(classfullNameElement);
                        classElement.Add(methodsList);

                        return classElement;
                    });
                });

                moduleElement.Add(fullNameElement);
                moduleElement.Add(moduleNameElement);
                moduleElement.Add(filesElement);
                moduleElement.Add(classesElement);

                return moduleElement;
            });

            coverageElement.Add(modulesListElement);

            document.Add(coverageElement);

            File.WriteAllText(output, document.ToString());
        }
    }
}