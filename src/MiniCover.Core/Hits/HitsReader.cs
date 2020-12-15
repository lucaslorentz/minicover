using System.Collections.Generic;
using System.IO;
using MiniCover.HitServices;

namespace MiniCover.Core.Hits
{
    public class HitsReader : IHitsReader
    {
        public HitsInfo TryReadFromDirectory(string path)
        {
            var contexts = new List<HitContext>();

            if (Directory.Exists(path))
            {
                foreach (var hitFile in Directory.GetFiles(path, "*.hits"))
                {
                    using (var fileStream = File.Open(hitFile, FileMode.Open, FileAccess.Read))
                    {
                        contexts.AddRange(HitContext.Deserialize(fileStream));
                    }
                }
            }

            return new HitsInfo(contexts);
        }
    }
}
