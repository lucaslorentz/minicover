using MiniCover.CommandLine.Options;
using MiniCover.UnitTests.TestHelpers;

namespace MiniCover.UnitTests.CommandLine.Options
{
    public class NCoverOutputOptionTests : FileOptionTests<OpenCoverOutputOption>
    {
        protected override string CurrentDirectory => "/current-directory".ToOSPath();
        protected override string ExpectedDefaultValue => "/current-directory/opencovercoverage.xml".ToOSPath();
        protected override string InputValue => "folder/file.xml".ToOSPath();
        protected override string ExpectedValue => "/current-directory/folder/file.xml".ToOSPath();

        protected override OpenCoverOutputOption Create()
        {
            return new OpenCoverOutputOption(MockFileSystem);
        }
    }
}
