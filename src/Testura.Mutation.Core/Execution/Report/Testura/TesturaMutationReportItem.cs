namespace Testura.Mutation.Core.Execution.Report.Testura
{
    public class TesturaMutationReportItem
    {
        public MutationOperators HeadCategory { get; set; }

        public string SubCategory { get; set; }

        public double MutationScore { get; set; }

        public int Survived { get; set; }

        public int Killed { get; set; }

        public int FailedOnCompilation { get; set; }
    }
}
