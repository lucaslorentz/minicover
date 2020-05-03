using MiniCover.CommandLine.Options;

namespace MiniCover.UnitTests.CommandLine.Options
{
    public class IncludeTestsPatternOptionTests : FilesPatternOptionTests<IncludeTestsPatternOption>
    {
        protected override string[] ExpectedDefaultValue => new string[] { "tests/**/*.cs", "test/**/*.cs" };
        protected override string[] InputValue => new string[] { "a.cs", "**/a.cs" };
        protected override string[] ExpectedValue => new string[] { "a.cs", "**/a.cs" };

        protected override IncludeTestsPatternOption Create()
        {
            return new IncludeTestsPatternOption();
        }
    }
}
