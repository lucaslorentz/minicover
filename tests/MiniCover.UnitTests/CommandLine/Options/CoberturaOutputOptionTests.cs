using MiniCover.CommandLine.Options;
using MiniCover.TestHelpers;

namespace MiniCover.UnitTests.CommandLine.Options
{
    public class CoberturaOutputOptionTests : FileOptionTests<CoberturaOutputOption>
    {
        protected override string CurrentDirectory => "/current-directory".ToOSPath();
        protected override string ExpectedDefaultValue => "/current-directory/cobertura.xml".ToOSPath();
        protected override string InputValue => "folder/file.xml".ToOSPath();
        protected override string ExpectedValue => "/current-directory/folder/file.xml".ToOSPath();

        protected override CoberturaOutputOption Create()
        {
            return new CoberturaOutputOption(MockFileSystem);
        }
    }
}
