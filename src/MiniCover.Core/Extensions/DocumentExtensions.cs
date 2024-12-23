using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Mono.Cecil.Cil;

namespace MiniCover.Core.Extensions
{
    public static class DocumentExtensions
    {
        public static bool FileHasChanged(this Document document)
        {
            if (!File.Exists(document.Url))
            {
                // Ignore not found source generated files because they might be in memory
                if (document.Url.EndsWith(".g.cs"))
                    return false;

                return true;
            }

            using (var stream = File.OpenRead(document.Url))
            {
                var hasher = CreateHashAlgorithm(document.HashAlgorithm);

                if (hasher == null)
                    return false;

                using (hasher)
                {
                    var newHash = hasher.ComputeHash(stream);
                    return !newHash.SequenceEqual(document.Hash);
                }
            }
        }

        private static HashAlgorithm CreateHashAlgorithm(DocumentHashAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case DocumentHashAlgorithm.SHA1: return SHA1.Create();
                case DocumentHashAlgorithm.SHA256: return SHA256.Create();
                case DocumentHashAlgorithm.MD5: return MD5.Create();
                default: return null;
            }
        }
    }
}
