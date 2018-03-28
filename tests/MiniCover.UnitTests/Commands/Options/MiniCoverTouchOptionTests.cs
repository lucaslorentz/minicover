using Microsoft.Extensions.CommandLineUtils;
using MiniCover.Commands.Options;
using System.Collections.Generic;
using System.IO;
using MiniCover.Commands.Options.Reports;
using Xunit;

namespace MiniCover.UnitTests.Commands.Options
{
    public class MiniCoverTouchOptionTests
    {
        [Theory]
        [MemberData(nameof(MiniCoverOptionsData))]
        internal void TouchOption_DefaultValue_CreationTest(MiniCoverTouchOption option, string expected)
        {
            //arange
            var cmd = new CommandLineApplication();
            option.AddTo(cmd);

            //act
            option.Validate();
            var actual = option.GetValue();
            var fileInfo = new FileInfo(actual);

            //assert
            Assert.Equal(expected, fileInfo.FullName);
            Assert.True(fileInfo.Exists);
        }

        public static IEnumerable<object[]> MiniCoverOptionsData
        {
            get
            {
                {
                    yield return new object[] { new CloverOutputOption(), Path.Combine(Directory.GetCurrentDirectory(), "clover.xml") };
                    yield return new object[] { new OpenCoverOutputOption(), Path.Combine(Directory.GetCurrentDirectory(), "opencovercoverage.xml") };
                    yield return new object[] { new NCoverOutputOption(), Path.Combine(Directory.GetCurrentDirectory(), "coverage.xml") };
                }
            }
        }

        [Theory]
        [MemberData(nameof(MiniCoverCustomOptionsData))]
        internal void TouchOption_CustomValue_CreationTest(MiniCoverTouchOption option, string argument, string expected)
        {
            //arange
            var cmd = new CommandLineApplication();
            option.AddTo(cmd);
            cmd.Options[0].Values.Add(argument);
            
            //act
            option.Validate();
            var actual = option.GetValue();
            var fileInfo = new FileInfo(actual);

            //assert
            Assert.Equal(expected, fileInfo.FullName);
            Assert.True(fileInfo.Exists);
        }

        public static IEnumerable<object[]> MiniCoverCustomOptionsData
        {
            get
            {
                {
                    yield return new object[] { new CloverOutputOption(), "cover1.xml", Path.Combine(Directory.GetCurrentDirectory(), "cover1.xml") };
                    yield return new object[] { new OpenCoverOutputOption(), "cover2.xml", Path.Combine(Directory.GetCurrentDirectory(), "cover2.xml") };
                    yield return new object[] { new NCoverOutputOption(), "cover3.xml", Path.Combine(Directory.GetCurrentDirectory(), "cover3.xml") };
                }
            }
        }
    }
}