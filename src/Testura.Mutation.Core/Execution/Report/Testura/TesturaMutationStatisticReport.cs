using System.Collections.Generic;

namespace Testura.Mutation.Core.Execution.Report.Testura
{
    public class TesturaMutationStatisticReport
    {
        public TesturaMutationStatisticReport()
        {
            All = new List<TesturaMutationReportItem>();
            Constructors = new List<TesturaMutationReportItem>();
            Methods = new List<TesturaMutationReportItem>();
            Properties = new List<TesturaMutationReportItem>();
        }

        public List<TesturaMutationReportItem> All { get; set; }

        public List<TesturaMutationReportItem> Constructors { get; set; }

        public List<TesturaMutationReportItem> Methods { get; set; }

        public List<TesturaMutationReportItem> Properties { get; set; }
    }
}
