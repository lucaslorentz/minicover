using MiniCover.CommandLine.Options;
using MiniCover.TestHelpers;

namespace MiniCover.UnitTests.CommandLine.Options
{
    public class ParentDirectoryOptionTests : DirectoryOptionTests<ParentDirectoryOption>
    {
        protected override string CurrentDirectory => "/current-directory".ToOSPath();
        protected override string ExpectedDefaultValue => "/current-directory".ToOSPath();
        protected override string InputValue => "folder".ToOSPath();
        protected override string ExpectedValue => "/current-directory/folder".ToOSPath();

        protected override ParentDirectoryOption Create()
        {
            return new ParentDirectoryOption(MockFileSystem);
        }
    }
}
