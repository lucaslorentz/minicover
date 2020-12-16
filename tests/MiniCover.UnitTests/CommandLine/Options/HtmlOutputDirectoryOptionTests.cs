using MiniCover.CommandLine.Options;
using MiniCover.TestHelpers;

namespace MiniCover.UnitTests.CommandLine.Options
{
    public class HtmlOutputDirectoryOptionTests : DirectoryOptionTests<HtmlOutputDirectoryOption>
    {
        protected override string CurrentDirectory => "/current-directory".ToOSPath();
        protected override string ExpectedDefaultValue => "/current-directory/coverage-html".ToOSPath();
        protected override string InputValue => "folder".ToOSPath();
        protected override string ExpectedValue => "/current-directory/folder".ToOSPath();

        protected override HtmlOutputDirectoryOption Create()
        {
            return new HtmlOutputDirectoryOption(MockFileSystem);
        }
    }
}
