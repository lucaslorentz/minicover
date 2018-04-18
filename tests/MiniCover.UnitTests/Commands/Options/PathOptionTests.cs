using Microsoft.Extensions.CommandLineUtils;
using MiniCover.Commands.Options;
using System.Collections.Generic;
using Xunit;

namespace MiniCover.UnitTests.Commands.Options
{
    public class PathOptionTests
    {
        [Theory]
        [MemberData(nameof(PathOptionsData))]
        internal void PathOption_DefaultValue_CreationTest(PathOption option, string expected)
        {
            //arange
            var cmd = new CommandLineApplication();
            option.AddTo(cmd);

            //act
            option.Validate();
            var actual = option.GetValue();

            //assert
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> PathOptionsData
        {
            get
            {
                {
                    yield return new object[] { new CoverageFileOption(), "./coverage.json" };
                    yield return new object[] { new CoverageHitsFileOption(), "./coverage-hits.txt" };
                }
            }
        }

        [Theory]
        [MemberData(nameof(MiniCoverCustomOptionsData))]
        internal void PathOption_CustomValue_CreationTest(PathOption option, string argument, string expected)
        {
            //arange
            var cmd = new CommandLineApplication();
            option.AddTo(cmd);
            cmd.Options[0].Values.Add(argument);

            //act
            option.Validate();
            var actual = option.GetValue();

            //assert
            Assert.Equal(expected, actual);
        }

        public static IEnumerable<object[]> MiniCoverCustomOptionsData
        {
            get
            {
                {
                    yield return new object[] { new CoverageFileOption(), "arg1.ex", "arg1.ex" };
                    yield return new object[] { new CoverageHitsFileOption(), "arg2.ex", "arg2.ex" };
                }
            }
        }
    }
}