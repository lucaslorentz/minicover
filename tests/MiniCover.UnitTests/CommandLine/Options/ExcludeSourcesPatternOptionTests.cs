using MiniCover.CommandLine.Options;

namespace MiniCover.UnitTests.CommandLine.Options
{
    public class ExcludeSourcesPatternOptionTests : FilesPatternOptionTests<ExcludeSourcesPatternOption>
    {
        protected override string[] ExpectedDefaultValue => new string[] { "**/bin/**/*.cs", "**/obj/**/*.cs" };
        protected override string[] InputValue => new string[] { "a.cs", "**/a.cs" };
        protected override string[] ExpectedValue => new string[] { "a.cs", "**/a.cs" };

        protected override ExcludeSourcesPatternOption Create()
        {
            return new ExcludeSourcesPatternOption();
        }
    }
}
