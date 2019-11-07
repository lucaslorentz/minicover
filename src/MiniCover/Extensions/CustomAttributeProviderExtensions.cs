using System.Linq;
using Mono.Cecil;

namespace MiniCover.Extensions
{
    public static class CustomAttributeProviderExtensions
    {
        public static bool HasCompilerGeneratedAttribute(this ICustomAttributeProvider definition)
        {
            return definition.CustomAttributes
                .Any(c => c.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute");
        }

        public static bool HasExcludeFromCodeCoverageAttribute(this ICustomAttributeProvider definition)
        {
            return definition.CustomAttributes
                .Any(a => a.AttributeType.FullName == "System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverageAttribute");
        }
    }
}
