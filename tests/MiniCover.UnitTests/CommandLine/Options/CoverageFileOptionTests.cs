using MiniCover.CommandLine.Options;
using MiniCover.TestHelpers;

namespace MiniCover.UnitTests.CommandLine.Options
{
    public class CoverageFileOptionTests : FileOptionTests<CoverageFileOption>
    {
        protected override string CurrentDirectory => "/current-directory".ToOSPath();
        protected override string ExpectedDefaultValue => "/current-directory/coverage.json".ToOSPath();
        protected override string InputValue => "folder/file.json".ToOSPath();
        protected override string ExpectedValue => "/current-directory/folder/file.json".ToOSPath();

        protected override CoverageFileOption Create()
        {
            return new CoverageFileOption(MockFileSystem);
        }
    }
}
