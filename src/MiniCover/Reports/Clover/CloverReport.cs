using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using MiniCover.Model;

namespace MiniCover.Reports.Clover
{
    public static class CloverReport
    {
        public static void Execute(InstrumentationResult result, FileInfo output, float threshold)
        {
            var hits = HitsInfo.TryReadFromDirectory(result.HitsPath);

            var document = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                CreateCoverageElement(result, hits)
            );

            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true
            };

            output.Directory.Create();

            using (var sw = output.CreateText())
            using (var writer = XmlWriter.Create(sw, xmlWriterSettings))
            {
                document.WriteTo(writer);
            }
        }

        private static XElement CreateCoverageElement(InstrumentationResult result, HitsInfo hits)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            return new XElement(
                XName.Get("coverage"),
                new XAttribute(XName.Get("generated"), timestamp),
                new XAttribute(XName.Get("clover"), "4.1.0"),
                CreateProjectElement(result, timestamp, hits)
            );
        }

        private static XElement CreateProjectElement(InstrumentationResult result, long timestamp, HitsInfo hits)
        {
            return new XElement(
                XName.Get("project"),
                new XAttribute(XName.Get("timestamp"), timestamp),
                new XAttribute(XName.Get("name"), result.SourcePath),
                CreateMetricsElement(CountProjectMetrics(result, hits)),
                CreatePackagesElement(result, hits)
            );
        }

        private static IEnumerable<XElement> CreatePackagesElement(InstrumentationResult result, HitsInfo hits)
        {
            return result.Assemblies.Select(assembly => new XElement(
                XName.Get("package"),
                new XAttribute(XName.Get("name"), assembly.Name),
                CreateMetricsElement(CountPackageMetrics(assembly, hits)),
                CreateFilesElement(assembly, hits)
            ));
        }

        private static IEnumerable<XElement> CreateFilesElement(InstrumentedAssembly assembly, HitsInfo hits)
        {
            return assembly.SourceFiles.Select(file => new XElement(
                XName.Get("file"),
                new XAttribute(XName.Get("name"), Path.GetFileName(file.Key)),
                new XAttribute(XName.Get("path"), file.Key),
                CreateMetricsElement(CountFileMetrics(file.Value, hits)),
                CreateClassesElement(file.Value.Instructions, hits),
                CreateLinesElement(file.Value.Instructions, hits)
            ));
        }

        private static IEnumerable<XElement> CreateClassesElement(IEnumerable<InstrumentedInstruction> instructions, HitsInfo hits)
        {
            return instructions
                .GroupBy(instruction => instruction.Method.Class)
                .Select(classes => new XElement(
                    XName.Get("class"),
                    new XAttribute(XName.Get("name"), classes.Key),
                    CreateMetricsElement(CountMetrics(classes, hits))
                ));
        }

        private static IEnumerable<XElement> CreateLinesElement(IEnumerable<InstrumentedInstruction> instructions, HitsInfo hits)
        {
            return instructions
                .SelectMany(t => t.GetLines(), (i, l) => new { instructionId = i.Id, line = l })
                .Select(instruction => new XElement(
                    XName.Get("line"),
                    new XAttribute(XName.Get("num"), instruction.line),
                    new XAttribute(XName.Get("count"), hits.GetInstructionHitCount(instruction.instructionId)),
                    new XAttribute(XName.Get("type"), "stmt")
                ));
        }

        private static XElement CreateMetricsElement(CloverCounter counter)
        {
            var element = new XElement(
                XName.Get("metrics"),
                new XAttribute(XName.Get("statements"), counter.Statements),
                new XAttribute(XName.Get("coveredstatements"), counter.CoveredStatements),
                new XAttribute(XName.Get("conditionals"), counter.Conditionals),
                new XAttribute(XName.Get("coveredconditionals"), counter.CoveredConditionals),
                new XAttribute(XName.Get("methods"), counter.Methods),
                new XAttribute(XName.Get("coveredmethods"), counter.CoveredMethods),
                new XAttribute(XName.Get("elements"), counter.Elements),
                new XAttribute(XName.Get("coveredelements"), counter.CoveredElements)
            );

            if (counter.Lines > 0)
            {
                element.Add(new XAttribute(XName.Get("loc"), counter.Lines));
                element.Add(new XAttribute(XName.Get("ncloc"), counter.Lines));
            }
            if (counter.Classes > 0)
            {
                element.Add(new XAttribute(XName.Get("classes"), counter.Classes));
            }
            if (counter.Files > 0)
            {
                element.Add(new XAttribute(XName.Get("files"), counter.Files));
            }
            if (counter.Packages > 0)
            {
                element.Add(new XAttribute(XName.Get("packages"), counter.Packages));
            }

            return element;
        }

        private static CloverCounter CountProjectMetrics(InstrumentationResult result, HitsInfo hits)
        {
            return result.Assemblies
                .Select(t => CountPackageMetrics(t, hits))
                .Aggregate(new CloverCounter(), (counter, next) =>
                {
                    counter.Add(next);
                    counter.Packages += 1;
                    return counter;
                }); ;
        }

        private static CloverCounter CountPackageMetrics(InstrumentedAssembly assembly, HitsInfo hits)
        {
            return assembly.SourceFiles
                .Select(t => CountFileMetrics(t.Value, hits))
                .Aggregate(new CloverCounter(), (counter, next) =>
                {
                    counter.Add(next);
                    counter.Files += 1;
                    return counter;
                });
        }

        private static CloverCounter CountFileMetrics(SourceFile file, HitsInfo hits)
        {
            var counter = CountMetrics(file.Instructions, hits);
            counter.Lines = file.Instructions.Max(instruction => instruction.EndLine);
            counter.Classes = file.Instructions.GroupBy(t => t.Method.Class).Count();
            return counter;
        }

        private static CloverCounter CountMetrics(IEnumerable<InstrumentedInstruction> instructions, HitsInfo hits)
        {
            var localInstructions = instructions.ToArray();
            var coveredInstructions = localInstructions
                .Where(instruction => hits.IsInstructionHit(instruction.Id)).ToArray();

            return new CloverCounter
            {
                Statements = localInstructions.Length,
                CoveredStatements = coveredInstructions.Length,
                Methods = localInstructions
                    .GroupBy(instruction => instruction.Method.FullName)
                    .Count(),
                CoveredMethods = coveredInstructions
                    .GroupBy(instruction => instruction.Method.FullName)
                    .Count()
            };
        }
    }
}