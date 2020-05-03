using MiniCover.CommandLine.Options;

namespace MiniCover.UnitTests.CommandLine.Options
{
    public class ExcludeTestsPatternOptionTests : FilesPatternOptionTests<ExcludeTestsPatternOption>
    {
        protected override string[] ExpectedDefaultValue => new string[] { "**/bin/**/*.cs", "**/obj/**/*.cs" };
        protected override string[] InputValue => new string[] { "a.cs", "**/a.cs" };
        protected override string[] ExpectedValue => new string[] { "a.cs", "**/a.cs" };

        protected override ExcludeTestsPatternOption Create()
        {
            return new ExcludeTestsPatternOption();
        }
    }
}
