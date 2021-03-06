using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using MiniCover.Core.Hits;
using MiniCover.Core.Model;

namespace MiniCover.Reports.Clover
{
    public class CloverReport : ICloverReport
    {
        private readonly IHitsReader _hitsReader;

        public CloverReport(IHitsReader hitsReader)
        {
            _hitsReader = hitsReader;
        }

        public void Execute(InstrumentationResult result, IFileInfo output)
        {
            var hits = _hitsReader.TryReadFromDirectory(result.HitsPath);

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
                new XAttribute(XName.Get("name"), Path.GetFileName(file.Path)),
                new XAttribute(XName.Get("path"), file.Path),
                CreateMetricsElement(CountFileMetrics(file, hits)),
                CreateClassesElement(file.Sequences, hits),
                CreateLinesElement(file.Sequences, hits)
            ));
        }

        private static IEnumerable<XElement> CreateClassesElement(IEnumerable<InstrumentedSequence> instructions, HitsInfo hits)
        {
            return instructions
                .GroupBy(instruction => instruction.Method.Class)
                .Select(classes => new XElement(
                    XName.Get("class"),
                    new XAttribute(XName.Get("name"), classes.Key),
                    CreateMetricsElement(CountMetrics(classes, hits))
                ));
        }

        private static IEnumerable<XElement> CreateLinesElement(IEnumerable<InstrumentedSequence> instructions, HitsInfo hits)
        {
            return instructions
                .SelectMany(t => t.GetLines(), (i, l) => new { instructionId = i.HitId, line = l })
                .Select(instruction => new XElement(
                    XName.Get("line"),
                    new XAttribute(XName.Get("num"), instruction.line),
                    new XAttribute(XName.Get("count"), hits.GetHitCount(instruction.instructionId)),
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
                .Select(t => CountFileMetrics(t, hits))
                .Aggregate(new CloverCounter(), (counter, next) =>
                {
                    counter.Add(next);
                    counter.Files += 1;
                    return counter;
                });
        }

        private static CloverCounter CountFileMetrics(SourceFile file, HitsInfo hits)
        {
            var counter = CountMetrics(file.Sequences, hits);
            counter.Lines = file.Sequences.Max(instruction => instruction.EndLine);
            counter.Classes = file.Sequences.GroupBy(t => t.Method.Class).Count();
            return counter;
        }

        private static CloverCounter CountMetrics(IEnumerable<InstrumentedSequence> instructions, HitsInfo hits)
        {
            var localInstructions = instructions.ToArray();
            var coveredInstructions = localInstructions
                .Where(instruction => hits.WasHit(instruction.HitId)).ToArray();

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