using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using MiniCover.CommandLine.Options;
using MiniCover.UnitTests.TestHelpers;

namespace MiniCover.UnitTests.CommandLine.Options
{
    public class WorkingDirectoryOptionTests : DirectoryOptionTests<WorkingDirectoryOption>
    {
        protected override string CurrentDirectory => "/current-directory".ToOSPath();
        protected override string ExpectedDefaultValue => "/current-directory".ToOSPath();
        protected override string InputValue => "folder".ToOSPath();
        protected override string ExpectedValue => "/current-directory/folder".ToOSPath();

        protected override WorkingDirectoryOption Create()
        {
            return new WorkingDirectoryOption(
                NullLogger<WorkingDirectoryOption>.Instance,
                MockFileSystem);
        }

        protected override void VerifyDefaultValue(WorkingDirectoryOption sut)
        {
            base.VerifyDefaultValue(sut);
            MockFileSystem.Directory.GetCurrentDirectory().Should().Be(CurrentDirectory);
        }

        protected override void VerifyInputValue(WorkingDirectoryOption sut)
        {
            base.VerifyInputValue(sut);
            MockFileSystem.Directory.GetCurrentDirectory().Should().Be(ExpectedValue);
        }
    }
}
