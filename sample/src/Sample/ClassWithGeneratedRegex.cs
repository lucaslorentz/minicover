using System.Text.RegularExpressions;

namespace Sample
{
    // This class is used to validate that Minicover can instrument assemblies containing code generated by a source generator.
    public static partial class ClassWithGeneratedRegex
    {
        [GeneratedRegex($@"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase)]
        private static partial Regex EmailRegex();

        public static bool IsEmail(string text)
        {
            return EmailRegex().IsMatch(text);
        }
    }
}