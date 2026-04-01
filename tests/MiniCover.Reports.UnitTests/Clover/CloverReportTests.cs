using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Xml.Linq;
using FluentAssertions;
using MiniCover.Core.Hits;
using MiniCover.Core.Model;
using MiniCover.HitServices;
using MiniCover.Reports.Clover;
using MiniCover.Reports.UnitTests.TestHelpers;
using Moq;
using Xunit;

namespace MiniCover.Reports.UnitTests.Clover
{
    public class CloverReportTests : TestBase
    {
        [Fact]
        public void Execute_ShouldIncludeConditionalMetricsFromBranches()
        {
            var method = new InstrumentedMethod
            {
                Class = "Sample.Feature",
                Name = "Run",
                FullName = "Sample.Feature.Run()"
            };

            var assembly = new InstrumentedAssembly("Sample");
            assembly.AddSequence("src/Sample.cs", new InstrumentedSequence
            {
                HitId = 10,
                StartLine = 42,
                EndLine = 42,
                StartColumn = 1,
                EndColumn = 10,
                Method = method,
                Conditions =
                [
                    new InstrumentedCondition
                    {
                        Branches =
                        [
                            new InstrumentedBranch { HitId = 11 },
                            new InstrumentedBranch { HitId = 12 }
                        ]
                    }
                ]
            });

            var result = new InstrumentationResult
            {
                SourcePath = "/repo",
                HitsPath = "/repo/coverage-hits"
            };
            result.AddInstrumentedAssembly(assembly);

            var hits = new HitsInfo(
            [
                new HitContext(
                    "Tests",
                    "Sample.Tests",
                    "Run",
                    new Dictionary<int, int>
                    {
                        { 10, 1 },
                        { 11, 1 }
                    })
            ]);

            var hitsReader = MockFor<IHitsReader>();
            hitsReader.Setup(reader => reader.TryReadFromDirectory(result.HitsPath)).Returns(hits);

            var sut = new CloverReport(hitsReader.Object);
            var outputPath = Path.GetTempFileName();
            var fileSystem = new FileSystem();

            try
            {
                sut.Execute(result, fileSystem.FileInfo.New(outputPath));

                var document = XDocument.Load(outputPath);
                var metrics = document
                    .Descendants("metrics")
                    .First();

                metrics.Attribute("statements")!.Value.Should().Be("1");
                metrics.Attribute("coveredstatements")!.Value.Should().Be("1");
                metrics.Attribute("conditionals")!.Value.Should().Be("2");
                metrics.Attribute("coveredconditionals")!.Value.Should().Be("1");
                metrics.Attribute("methods")!.Value.Should().Be("1");
                metrics.Attribute("coveredmethods")!.Value.Should().Be("1");
                metrics.Attribute("elements")!.Value.Should().Be("4");
                metrics.Attribute("coveredelements")!.Value.Should().Be("3");
            }
            finally
            {
                File.Delete(outputPath);
            }
        }
    }
}
