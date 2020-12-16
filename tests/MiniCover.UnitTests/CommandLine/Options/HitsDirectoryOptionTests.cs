using MiniCover.CommandLine.Options;
using MiniCover.TestHelpers;

namespace MiniCover.UnitTests.CommandLine.Options
{
    public class HitsDirectoryOptionTests : DirectoryOptionTests<HitsDirectoryOption>
    {
        protected override string CurrentDirectory => "/current-directory".ToOSPath();
        protected override string ExpectedDefaultValue => "/current-directory/coverage-hits".ToOSPath();
        protected override string InputValue => "folder".ToOSPath();
        protected override string ExpectedValue => "/current-directory/folder".ToOSPath();

        protected override HitsDirectoryOption Create()
        {
            return new HitsDirectoryOption(MockFileSystem);
        }
    }
}
