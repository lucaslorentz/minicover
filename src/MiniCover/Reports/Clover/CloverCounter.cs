namespace MiniCover.Reports.Clover
{
    public class CloverCounter
    {
        public int Statements { get; set; }

        public int CoveredStatements { get; set; }

        public int Conditionals { get; set; }

        public int CoveredConditionals { get; set; }

        public int Methods { get; set; }

        public int CoveredMethods { get; set; }

        public int Lines { get; set; }

        public int Classes { get; set; }

        public int Files { get; set; }

        public int Packages { get; set; }

        public int Elements => Statements + Conditionals + Methods;

        public int CoveredElements => CoveredStatements + CoveredConditionals + CoveredMethods;

        public void Add(CloverCounter counter)
        {
            Statements += counter.Statements;
            CoveredStatements += counter.CoveredStatements;
            Conditionals += counter.Conditionals;
            CoveredConditionals += counter.CoveredConditionals;
            Methods += counter.Methods;
            CoveredMethods += counter.CoveredMethods;
            Lines += counter.Lines;
            Classes += counter.Classes;
            Files += counter.Files;
            Packages += counter.Packages;
        }
    }
}