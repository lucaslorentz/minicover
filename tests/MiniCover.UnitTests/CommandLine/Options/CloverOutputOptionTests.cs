using MiniCover.CommandLine.Options;
using MiniCover.TestHelpers;

namespace MiniCover.UnitTests.CommandLine.Options
{
    public class CloverOutputOptionTests : FileOptionTests<CloverOutputOption>
    {
        protected override string CurrentDirectory => "/current-directory".ToOSPath();
        protected override string ExpectedDefaultValue => "/current-directory/clover.xml".ToOSPath();
        protected override string InputValue => "folder/file.xml".ToOSPath();
        protected override string ExpectedValue => "/current-directory/folder/file.xml".ToOSPath();

        protected override CloverOutputOption Create()
        {
            return new CloverOutputOption(MockFileSystem);
        }
    }
}
