using MiniCover.Core.Model;

namespace MiniCover.Reports.Helpers
{
    public class SummaryRow
    {
        public int Level { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public bool Root { get; set; }
        public bool Folder { get; set; }
        public bool File { get; set; }
        public SourceFile[] SourceFiles { get; set; }
        public Summary Summary { get; set; }
    }
}
