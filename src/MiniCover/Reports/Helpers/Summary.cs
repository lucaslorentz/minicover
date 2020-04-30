namespace MiniCover.Reports.Helpers
{

    public class Summary
    {
        public int Statements { get; set; }
        public int CoveredStatements { get; set; }
        public float StatementsPercentage { get; set; }
        public bool StatementsCoveragePass { get; set; }
        public int Lines { get; set; }
        public int CoveredLines { get; set; }
        public float LinesPercentage { get; set; }
        public bool LinesCoveragePass { get; set; }
        public int Branches { get; set; }
        public int CoveredBranches { get; set; }
        public float BranchesPercentage { get; set; }
        public bool BranchesCoveragePass { get; set; }
    }
}
