using MiniCover.CommandLine.Options;
using MiniCover.TestHelpers;

namespace MiniCover.UnitTests.CommandLine.Options
{
    public class OpenCoverOutputOptionTests : FileOptionTests<NCoverOutputOption>
    {
        protected override string CurrentDirectory => "/current-directory".ToOSPath();
        protected override string ExpectedDefaultValue => "/current-directory/coverage.xml".ToOSPath();
        protected override string InputValue => "folder/file.xml".ToOSPath();
        protected override string ExpectedValue => "/current-directory/folder/file.xml".ToOSPath();

        protected override NCoverOutputOption Create()
        {
            return new NCoverOutputOption(MockFileSystem);
        }
    }
}
