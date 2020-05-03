using MiniCover.CommandLine.Options;

namespace MiniCover.UnitTests.CommandLine.Options
{
    public class ExcludeAssembliesPatternOptionTests : FilesPatternOptionTests<ExcludeAssembliesPatternOption>
    {
        protected override string[] ExpectedDefaultValue => new string[] { "**/obj/**/*.dll" };
        protected override string[] InputValue => new string[] { "a.dll", "**/a.dll" };
        protected override string[] ExpectedValue => new string[] { "a.dll", "**/a.dll" };

        protected override ExcludeAssembliesPatternOption Create()
        {
            return new ExcludeAssembliesPatternOption();
        }
    }
}
