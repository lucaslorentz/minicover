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
        public static void Execute(InstrumentationResult result, string output, float threshold)
        {
            var hits = File.Exists(result.HitsFile)
                ? File.ReadAllLines(result.HitsFile)
                    .Select(h => int.Parse(h))
                    .GroupBy(h => h)
                    .ToDictionary(h => h.Key, h => h.Count())
                : new Dictionary<int, int>();

            var document = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                CreateCoverageElement(result, hits)
            );

            var xmlWriterSettings = new XmlWriterSettings
            {
                Indent = true
            };

            var path = Path.GetDirectoryName(output);

            if(!string.IsNullOrEmpty(path))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(output));
            }

            using (StreamWriter sw = File.CreateText(output))
            using (XmlWriter writer = XmlWriter.Create(sw, xmlWriterSettings))
            {
                document.WriteTo(writer);
            }
        }

        private static XElement CreateCoverageElement(InstrumentationResult result, IDictionary<int, int> hits)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            return new XElement(
                XName.Get("coverage"),
                new XAttribute(XName.Get("generated"), timestamp),
                new XAttribute(XName.Get("clover"), "4.1.0"),
                CreateProjectElement(result, timestamp, hits)
            );
        }

        private static XElement CreateProjectElement(InstrumentationResult result, long timestamp, IDictionary<int, int> hits)
        {
            return new XElement(
                XName.Get("project"),
                new XAttribute(XName.Get("timestamp"), timestamp),
                new XAttribute(XName.Get("name"), result.SourcePath),
                CreateMetricsElement(CountProjectMetrics(result, hits)),
                CreatePackagesElement(result, hits)
            );
        }

        private static IEnumerable<XElement> CreatePackagesElement(InstrumentationResult result, IDictionary<int, int> hits)
        {
            return result.Assemblies.Select(assembly => new XElement(
                XName.Get("package"),
                new XAttribute(XName.Get("name"), assembly.Name),
                CreateMetricsElement(CountPackageMetrics(assembly, hits)),
                CreateFilesElement(assembly, hits)
            ));
        }

        private static IEnumerable<XElement> CreateFilesElement(InstrumentedAssembly assembly, IDictionary<int, int> hits)
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

        private static IEnumerable<XElement> CreateClassesElement(IEnumerable<InstrumentedInstruction> instructions, IDictionary<int, int> hits)
        {
            return instructions
                .GroupBy(instruction => instruction.Class)
                .Select(classes => new XElement(
                    XName.Get("class"),
                    new XAttribute(XName.Get("name"), classes.Key),
                    CreateMetricsElement(CountMetrics(classes, hits))
                ));
        }

        private static IEnumerable<XElement> CreateLinesElement(IEnumerable<InstrumentedInstruction> instructions, IDictionary<int, int> hits)
        {
            return instructions
                .SelectMany(t => t.GetLines(), (i, l) => new { instructionId = i.Id, line = l })
                .Select(instruction => new XElement(
                    XName.Get("line"),
                    new XAttribute(XName.Get("num"), instruction.line),
                    new XAttribute(XName.Get("count"), hits.TryGetValue(instruction.instructionId, out int count) ? count : 0),
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

            if(counter.Lines > 0)
            {
                element.Add(new XAttribute(XName.Get("loc"), counter.Lines));
                element.Add(new XAttribute(XName.Get("ncloc"), counter.Lines));
            }
            if(counter.Classes > 0)
            {
                element.Add(new XAttribute(XName.Get("classes"), counter.Classes));
            }
            if(counter.Files > 0)
            {
                element.Add(new XAttribute(XName.Get("files"), counter.Files));
            }
            if(counter.Packages > 0)
            {
                element.Add(new XAttribute(XName.Get("packages"), counter.Packages));
            }

            return element;
        }

        private static CloverCounter CountProjectMetrics(InstrumentationResult result, IDictionary<int, int> hits)
        {
            return result.Assemblies
                .Select(t => CountPackageMetrics(t, hits))
                .Aggregate(new CloverCounter(), (counter, next) =>
                {
                    counter.Add(next);
                    counter.Packages += 1;
                    return counter;
                });;
        }

        private static CloverCounter CountPackageMetrics(InstrumentedAssembly assembly, IDictionary<int, int> hits)
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

        private static CloverCounter CountFileMetrics(SourceFile file, IDictionary<int, int> hits)
        {
            var counter = CountMetrics(file.Instructions, hits);
            counter.Lines = file.Instructions.Max(instruction => instruction.EndLine);
            counter.Classes = file.Instructions.GroupBy(t => t.Class).Count();
            return counter;
        }

        private static CloverCounter CountMetrics(IEnumerable<InstrumentedInstruction> instructions, IDictionary<int, int> hits)
        {
            var coveredInstructions = instructions
                .Where(instruction => hits.ContainsKey(instruction.Id))
                .ToList();

            return new CloverCounter
            {
                Statements = instructions.Count(),
                CoveredStatements = coveredInstructions.Count(),
                Methods = instructions
                    .GroupBy(instruction => instruction.MethodFullName)
                    .Count(),
                CoveredMethods = coveredInstructions
                    .GroupBy(instruction => instruction.MethodFullName)
                    .Count()
            };
        }
    }
}