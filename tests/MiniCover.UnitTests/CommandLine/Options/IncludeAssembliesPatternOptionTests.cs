using MiniCover.CommandLine.Options;

namespace MiniCover.UnitTests.CommandLine.Options
{
    public class IncludeAssembliesPatternOptionTests : FilesPatternOptionTests<IncludeAssembliesPatternOption>
    {
        protected override string[] ExpectedDefaultValue => new string[] { "**/*.dll" };
        protected override string[] InputValue => new string[] { "a.dll", "**/a.dll" };
        protected override string[] ExpectedValue => new string[] { "a.dll", "**/a.dll" };

        protected override IncludeAssembliesPatternOption Create()
        {
            return new IncludeAssembliesPatternOption();
        }
    }
}
