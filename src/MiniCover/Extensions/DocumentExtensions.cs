using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Mono.Cecil.Cil;

namespace MiniCover.Extensions
{
    public static class DocumentExtensions
    {
        public static bool FileHasChanged(this Document document)
        {
            if (!File.Exists(document.Url))
                return true;

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
                case DocumentHashAlgorithm.SHA1: return new SHA1Managed();
                case DocumentHashAlgorithm.SHA256: return new SHA256Managed();
                case DocumentHashAlgorithm.MD5: return new MD5CryptoServiceProvider();
                default: return null;
            }
        }
    }
}
