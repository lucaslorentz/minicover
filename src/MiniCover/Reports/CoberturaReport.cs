using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using MiniCover.Extensions;
using MiniCover.Model;

namespace MiniCover.Reports
{
    public class CoberturaReport
    {
        public void Execute(InstrumentationResult result, FileInfo output)
        {
            var hits = HitsInfo.TryReadFromDirectory(result.HitsPath);

            var document = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XDocumentType("coverage", null, "http://cobertura.sourceforge.net/xml/coverage-04.dtd", null),
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

        private static XElement CreateCoverageElement(InstrumentationResult result, HitsInfo hitsInfo)
        {
            var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            var linesValid = result.GetSourceFiles()
                .SelectMany(kvFile => kvFile.Value.Instructions)
                .SelectMany(i => i.GetLines())
                .Distinct()
                .Count();

            var coveredLines = result.GetSourceFiles()
                .SelectMany(kvFile => kvFile.Value.Instructions)
                .Where(h => hitsInfo.IsInstructionHit(h.Id))
                .SelectMany(i => i.GetLines())
                .Distinct()
                .Count();

            var linesRate = linesValid == 0 ? 0d : (double)coveredLines / (double)linesValid;

            return new XElement(
                XName.Get("coverage"),
                new XAttribute(XName.Get("lines-valid"), linesValid),
                new XAttribute(XName.Get("lines-covered"), coveredLines),
                new XAttribute(XName.Get("line-rate"), linesRate),
                new XAttribute(XName.Get("timestamp"), timestamp),
                new XAttribute(XName.Get("version"), "1.0"),
                CrateSourcesElement(result, hitsInfo),
                CratePackagesElement(result, hitsInfo)
            );
        }

        private static XElement CrateSourcesElement(InstrumentationResult result, HitsInfo hitsInfo)
        {
            return new XElement(
                XName.Get("sources"),
                new XElement("source",
                    new XText(result.SourcePath)
                )
            );
        }

        private static XElement CratePackagesElement(InstrumentationResult result, HitsInfo hitsInfo)
        {
            return new XElement(
                XName.Get("packages"),
                result.Assemblies
                    .Where(a => a.SourceFiles.Count > 0)
                    .Select(a => CreatePackageElement(a, hitsInfo))
            );
        }

        private static XElement CreatePackageElement(InstrumentedAssembly assembly, HitsInfo hitsInfo)
        {
            var linesValid = assembly.SourceFiles
                .SelectMany(kvFile => kvFile.Value.Instructions)
                .SelectMany(i => i.GetLines())
                .Distinct()
                .Count();

            var coveredLines = assembly.SourceFiles
                .SelectMany(kvFile => kvFile.Value.Instructions)
                .Where(h => hitsInfo.IsInstructionHit(h.Id))
                .SelectMany(i => i.GetLines())
                .Distinct()
                .Count();

            var linesRate = linesValid == 0 ? 0d : (double)coveredLines / (double)linesValid;

            return new XElement(
                XName.Get("package"),
                new XAttribute(XName.Get("name"), assembly.Name),
                new XAttribute(XName.Get("line-rate"), linesRate),
                CreateClassesElement(assembly, hitsInfo)
            );
        }

        private static XElement CreateClassesElement(InstrumentedAssembly assembly, HitsInfo hitsInfo)
        {
            return new XElement(
                XName.Get("classes"),
                assembly.SourceFiles
                    .Select(kv => CreateClassElement(kv.Key, kv.Value, hitsInfo))
            );
        }

        private static XElement CreateClassElement(string fileName, SourceFile sourceFile, HitsInfo hitsInfo)
        {
            var linesValid = sourceFile.Instructions
                .SelectMany(i => i.GetLines())
                .Distinct()
                .Count();

            var coveredLines = sourceFile.Instructions
                .Where(h => hitsInfo.IsInstructionHit(h.Id))
                .SelectMany(i => i.GetLines())
                .Distinct()
                .Count();

            var linesRate = (double)coveredLines / (double)linesValid;

            return new XElement(

                XName.Get("class"),
                new XAttribute(XName.Get("name"), fileName),
                new XAttribute(XName.Get("filename"), fileName),
                new XAttribute(XName.Get("line-rate"), linesRate),
                CreateMethodsElement(sourceFile, hitsInfo),
                CreateLinesElement(sourceFile.Instructions, hitsInfo)
            );
        }

        private static XElement CreateMethodsElement(SourceFile sourceFile, HitsInfo hitsInfo)
        {
            return new XElement(
                XName.Get("methods"),
                sourceFile.Instructions
                    .GroupBy(i => i.Method)
                    .Select(g => CreateMethodElement(g.Key, g, hitsInfo))
            );
        }

        private static XElement CreateMethodElement(InstrumentedMethod method, IEnumerable<InstrumentedInstruction> instructions, HitsInfo hitsInfo)
        {
            var hits = instructions.Sum(i => hitsInfo.GetInstructionHitCount(i.Id));

            var openParametersIndex = method.FullName.IndexOf("(");
            var signature = method.FullName.Substring(openParametersIndex);

            return new XElement(
                XName.Get("method"),
                new XAttribute(XName.Get("name"), method.Name),
                new XAttribute(XName.Get("signature"), signature),
                new XAttribute(XName.Get("hits"), hits),
                CreateLinesElement(instructions, hitsInfo)
            );
        }

        private static XElement CreateLinesElement(IEnumerable<InstrumentedInstruction> instructions, HitsInfo hitsInfo)
        {
            return new XElement(
                XName.Get("lines"),
                instructions
                    .GroupByMany(i => i.GetLines())
                    .OrderBy(g => g.Key)
                    .Select(g => CreateLineElement(g.Key, g, hitsInfo))
            );
        }

        private static object CreateLineElement(int line, IEnumerable<InstrumentedInstruction> instructions, HitsInfo hitsInfo)
        {
            var hits = instructions.Sum(i => hitsInfo.GetInstructionHitCount(i.Id));

            return new XElement(
                XName.Get("line"),
                new XAttribute(XName.Get("number"), line),
                new XAttribute(XName.Get("hits"), hits)
            );
        }
    }
}
