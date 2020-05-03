using MiniCover.CommandLine.Options;

namespace MiniCover.UnitTests.CommandLine.Options
{
    public class IncludeSourcesPatternOptionTests : FilesPatternOptionTests<IncludeSourcesPatternOption>
    {
        protected override string[] ExpectedDefaultValue => new string[] { "src/**/*.cs" };
        protected override string[] InputValue => new string[] { "a.cs", "**/a.cs" };
        protected override string[] ExpectedValue => new string[] { "a.cs", "**/a.cs" };

        protected override IncludeSourcesPatternOption Create()
        {
            return new IncludeSourcesPatternOption();
        }
    }
}
