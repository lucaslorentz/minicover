using Microsoft.Extensions.CommandLineUtils;
using MiniCover.Commands.Options.FileParameterizations;
using MiniCover.Commands.Options.Reports;
using System.Collections.Generic;
using System.IO;
using MiniCover.Commands.Options;
using Xunit;

namespace MiniCover.UnitTests.Commands.Options
{
    public class ParameterizationOptionTests
    {
        [Theory]
        [MemberData(nameof(MiniCoverCustomOptionsData))]
        internal void ParameterizationOption_CustomValue_CreationTest(ParameterizationOption option, string argument, string expectedOpenCoverOutputPath, float expectedThreshold)
        {
            //arange
            var cmd = new CommandLineApplication();
            option.AddTo(cmd);
            cmd.Options[0].Values.Add(argument);
            var nestedOptions = option.NestedOptions();
            foreach (var miniCoverOption in nestedOptions)
            {
                miniCoverOption.AddTo(cmd);
            }

            foreach (var miniCoverOption in nestedOptions)
            {
                miniCoverOption.Validate();
            }

            var stream = File.CreateText(argument);
            stream.WriteLine($"Threshold: {expectedThreshold}");
            stream.Close();
            stream.Dispose();

            //act
            option.Validate();
            var actual = ((IMiniCoverOption<MiniCoverParameterization>)option).GetValue();

            //assert
            Assert.Equal(expectedOpenCoverOutputPath, actual.OpenCoverFile);
            Assert.Equal(expectedThreshold, actual.Threshold);
        }

        public static IEnumerable<object[]> MiniCoverCustomOptionsData
        {
            get
            {
                {
                    yield return new object[] { new ParameterizationOption(new OpenCoverOutputOption()), "file.yml", "./opencovercoverage.xml", 90 };
                }
            }
        }
    }
}