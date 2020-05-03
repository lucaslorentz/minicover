using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using MiniCover.Extensions;
using MiniCover.Model;

namespace MiniCover.Reports.Cobertura
{
    public class CoberturaReport
    {
        public void Execute(InstrumentationResult result, IFileInfo output)
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

            var allLines = result.GetSourceFiles()
                .SelectMany(file => file.Sequences)
                .SelectMany(i => i.GetLines())
                .Distinct()
                .Count();

            var coveredLines = result.GetSourceFiles()
                .SelectMany(file => file.Sequences)
                .Where(h => hitsInfo.WasHit(h.HitId))
                .SelectMany(i => i.GetLines())
                .Distinct()
                .Count();

            var allBranches = result.GetSourceFiles()
                .SelectMany(file => file.Sequences)
                .SelectMany(i => i.Conditions)
                .SelectMany(c => c.Branches)
                .Count();

            var coveredBranches = result.GetSourceFiles()
                .SelectMany(file => file.Sequences)
                .SelectMany(i => i.Conditions)
                .SelectMany(c => c.Branches)
                .Where(b => hitsInfo.WasHit(b.HitId))
                .Count();

            var lineRate = allLines == 0 ? 1d : (double)coveredLines / (double)allLines;
            var branchRate = allBranches == 0 ? 1d : (double)coveredBranches / (double)allBranches;

            return new XElement(
                XName.Get("coverage"),
                new XAttribute(XName.Get("lines-valid"), allLines),
                new XAttribute(XName.Get("lines-covered"), coveredLines),
                new XAttribute(XName.Get("line-rate"), lineRate),
                new XAttribute(XName.Get("branches-valid"), allBranches),
                new XAttribute(XName.Get("branches-covered"), coveredBranches),
                new XAttribute(XName.Get("branch-rate"), branchRate),
                new XAttribute(XName.Get("complexity"), 0),
                new XAttribute(XName.Get("timestamp"), timestamp),
                new XAttribute(XName.Get("version"), "1.0.0"),
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
                    .Where(a => a.SourceFiles.Any())
                    .Select(a => CreatePackageElement(a, hitsInfo))
            );
        }

        private static XElement CreatePackageElement(InstrumentedAssembly assembly, HitsInfo hitsInfo)
        {
            var allLines = assembly.SourceFiles
                .SelectMany(file => file.Sequences)
                .SelectMany(i => i.GetLines())
                .Distinct()
                .Count();

            var coveredLines = assembly.SourceFiles
                .SelectMany(file => file.Sequences)
                .Where(h => hitsInfo.WasHit(h.HitId))
                .SelectMany(i => i.GetLines())
                .Distinct()
                .Count();

            var allBranches = assembly.SourceFiles
                .SelectMany(file => file.Sequences)
                .SelectMany(i => i.Conditions)
                .SelectMany(c => c.Branches)
                .Count();

            var coveredBranches = assembly.SourceFiles
                .SelectMany(file => file.Sequences)
                .SelectMany(i => i.Conditions)
                .SelectMany(c => c.Branches)
                .Where(b => hitsInfo.WasHit(b.HitId))
                .Count();

            var lineRate = allLines == 0 ? 1d : (double)coveredLines / (double)allLines;
            var branchRate = allBranches == 0 ? 1d : (double)coveredBranches / (double)allBranches;

            return new XElement(
                XName.Get("package"),
                new XAttribute(XName.Get("name"), assembly.Name),
                new XAttribute(XName.Get("line-rate"), lineRate),
                new XAttribute(XName.Get("branch-rate"), branchRate),
                new XAttribute(XName.Get("complexity"), 0),
                CreateClassesElement(assembly, hitsInfo)
            );
        }

        private static XElement CreateClassesElement(InstrumentedAssembly assembly, HitsInfo hitsInfo)
        {
            return new XElement(
                XName.Get("classes"),
                assembly.SourceFiles
                    .Select(file => CreateClassElement(file, hitsInfo))
            );
        }

        private static XElement CreateClassElement(SourceFile sourceFile, HitsInfo hitsInfo)
        {
            var allLines = sourceFile.Sequences
                .SelectMany(i => i.GetLines())
                .Distinct()
                .Count();

            var coveredLines = sourceFile.Sequences
                .Where(h => hitsInfo.WasHit(h.HitId))
                .SelectMany(i => i.GetLines())
                .Distinct()
                .Count();

            var allBranches = sourceFile.Sequences
                .SelectMany(i => i.Conditions)
                .SelectMany(c => c.Branches)
                .Count();

            var coveredBranches = sourceFile.Sequences
                .SelectMany(i => i.Conditions)
                .SelectMany(c => c.Branches)
                .Where(b => hitsInfo.WasHit(b.HitId))
                .Count();

            var lineRate = allLines == 0 ? 1d : (double)coveredLines / (double)allLines;
            var branchRate = allBranches == 0 ? 1d : (double)coveredBranches / (double)allBranches;

            return new XElement(

                XName.Get("class"),
                new XAttribute(XName.Get("name"), sourceFile.Path),
                new XAttribute(XName.Get("filename"), sourceFile.Path),
                new XAttribute(XName.Get("line-rate"), lineRate),
                new XAttribute(XName.Get("branch-rate"), branchRate),
                new XAttribute(XName.Get("complexity"), 0),
                CreateMethodsElement(sourceFile, hitsInfo),
                CreateLinesElement(sourceFile.Sequences, hitsInfo)
            );
        }

        private static XElement CreateMethodsElement(SourceFile sourceFile, HitsInfo hitsInfo)
        {
            return new XElement(
                XName.Get("methods"),
                sourceFile.Sequences
                    .GroupBy(i => i.Method)
                    .Select(g => CreateMethodElement(g.Key, g, hitsInfo))
            );
        }

        private static XElement CreateMethodElement(InstrumentedMethod method, IEnumerable<InstrumentedSequence> instructions, HitsInfo hitsInfo)
        {
            var allLines = instructions
                .SelectMany(i => i.GetLines())
                .Distinct()
                .Count();

            var coveredLines = instructions
                .Where(h => hitsInfo.WasHit(h.HitId))
                .SelectMany(i => i.GetLines())
                .Distinct()
                .Count();

            var allBranches = instructions
                .SelectMany(i => i.Conditions)
                .SelectMany(c => c.Branches)
                .Count();

            var coveredBranches = instructions
                .SelectMany(i => i.Conditions)
                .SelectMany(c => c.Branches)
                .Where(b => hitsInfo.WasHit(b.HitId))
                .Count();

            var lineRate = allLines == 0 ? 1d : (double)coveredLines / (double)allLines;
            var branchRate = allBranches == 0 ? 1d : (double)coveredBranches / (double)allBranches;

            var openParametersIndex = method.FullName.IndexOf("(");
            var signature = method.FullName.Substring(openParametersIndex);

            return new XElement(
                XName.Get("method"),
                new XAttribute(XName.Get("name"), method.Name),
                new XAttribute(XName.Get("signature"), signature),
                new XAttribute(XName.Get("line-rate"), lineRate),
                new XAttribute(XName.Get("branch-rate"), branchRate),
                new XAttribute(XName.Get("complexity"), 0),
                CreateLinesElement(instructions, hitsInfo)
            );
        }

        private static XElement CreateLinesElement(IEnumerable<InstrumentedSequence> instructions, HitsInfo hitsInfo)
        {
            return new XElement(
                XName.Get("lines"),
                instructions
                    .GroupByMany(i => i.GetLines())
                    .OrderBy(g => g.Key)
                    .Select(g => CreateLineElement(g.Key, g, hitsInfo))
            );
        }

        private static XElement CreateLineElement(int line, IEnumerable<InstrumentedSequence> instructions, HitsInfo hitsInfo)
        {
            var conditions = instructions
                .SelectMany(i => i.Conditions)
                .ToArray();

            var allBranches = conditions
                .SelectMany(c => c.Branches)
                .Count();

            var coveredBranches = conditions
                .SelectMany(c => c.Branches)
                .Where(b => hitsInfo.WasHit(b.HitId))
                .Count();

            var hits = instructions.Sum(i => hitsInfo.GetHitCount(i.HitId));

            var conditionCoverage = allBranches == 0 ? 0 : coveredBranches * 100 / allBranches;

            return new XElement(
                XName.Get("line"),
                new XAttribute(XName.Get("number"), line),
                new XAttribute(XName.Get("hits"), hits),
                new XAttribute(XName.Get("branch"), allBranches > 0 ? "true" : "false"),
                new XAttribute(XName.Get("condition-coverage"), $"{conditionCoverage}% ({coveredBranches}/{allBranches})"),
                allBranches > 0
                    ? CreateConditionsElements(conditions, hitsInfo)
                    : null
            );
        }

        private static XElement CreateConditionsElements(IEnumerable<InstrumentedCondition> conditions, HitsInfo hitsInfo)
        {
            return new XElement(
                XName.Get("conditions"),
                conditions.Select((b, i) => CreateConditionElement(b, i, hitsInfo))
            );
        }

        private static XElement CreateConditionElement(InstrumentedCondition condition, int index, HitsInfo hitsInfo)
        {
            var allBranches = condition.Branches;

            var coveredBranches = allBranches
                .Where(b => hitsInfo.WasHit(b.HitId))
                .Count();

            var coverage = allBranches.Length == 0 ? 0 : coveredBranches * 100 / allBranches.Length;

            return new XElement(
                XName.Get("condition"),
                new XAttribute(XName.Get("number"), index),
                new XAttribute(XName.Get("type"), "jump"),
                new XAttribute(XName.Get("coverage"), $"{coverage}%")
            );
        }
    }
}
