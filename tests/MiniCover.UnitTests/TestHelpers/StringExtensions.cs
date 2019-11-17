namespace MiniCover.UnitTests.TestHelpers
{
    public static class StringExtensions
    {
        public static string NormalizeLineEndings(this string text)
        {
            return text.Replace("\r\n", "\n");
        }
    }
}
