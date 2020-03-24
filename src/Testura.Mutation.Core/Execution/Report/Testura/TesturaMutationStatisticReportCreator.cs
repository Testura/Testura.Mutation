using System;
using System.Collections.Generic;
using System.Linq;

namespace Testura.Mutation.Core.Execution.Report.Testura
{
    public class TesturaMutationStatisticReportCreator : ReportCreator
    {
        public TesturaMutationStatisticReportCreator(string savePath)
            : base(savePath)
        {
        }

        public override void SaveReport(IList<MutationDocumentResult> mutations, TimeSpan executionTime)
        {

        }

        private void CreateAll(IList<MutationDocumentResult> mutations)
        {
            var o = new List<TesturaMutationReportItem>();

            var mutationOperatorTypes = Enum.GetValues(typeof(MutationOperators)).Cast<MutationOperators>();
            foreach (var mutationOperatorType in mutationOperatorTypes)
            {
                var mutationByOperator = mutations.Where(m => m.Category.HeadCategory == mutationOperatorType);

                if (!mutationByOperator.Any())
                {
                    continue;
                }

                var mutationOperatorsBySubCategory = mutationByOperator.GroupBy(m => m.Category.Subcategory);

                foreach (var nn in mutationOperatorsBySubCategory)
                {
                    o.Add(new TesturaMutationReportItem
                    {
                        HeadCategory = mutationOperatorType,
                        SubCategory = 
                    }
                }
            }
        }
    }
}
