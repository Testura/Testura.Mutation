using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
            if (mutations == null || !mutations.Any())
            {
                return;
            }

            var report = new TesturaMutationStatisticReport
            {
                All = CreateMutationReportItems(mutations, null),
                Constructors = CreateMutationReportItems(mutations, LocationKind.Constructor),
                Methods = CreateMutationReportItems(mutations, LocationKind.Method),
                Properties = CreateMutationReportItems(mutations, LocationKind.Property)
            };

            using (var file = File.CreateText(SavePath))
            {
                var serializer = new JsonSerializer();
                serializer.Converters.Add(new StringEnumConverter());
                serializer.Serialize(file, report);
            }
        }

        private List<TesturaMutationReportItem> CreateMutationReportItems(IList<MutationDocumentResult> mutations, LocationKind? locationKind)
        {
            var mutationReportItems = new List<TesturaMutationReportItem>();

            var mutationOperatorTypes = Enum.GetValues(typeof(MutationOperators)).Cast<MutationOperators>();
            foreach (var mutationOperatorType in mutationOperatorTypes)
            {
                var mutationByOperator = mutations.Where(m => m.Category.HeadCategory == mutationOperatorType);

                if (!mutationByOperator.Any())
                {
                    continue;
                }

                if (locationKind != null)
                {
                    mutationByOperator = mutations.Where(m => m.Location.Kind == locationKind);
                }

                var mutationOperatorsBySubCategories = mutationByOperator.GroupBy(m => m.Category.Subcategory);
                foreach (var mutationOperatorBySubCategory in mutationOperatorsBySubCategories)
                {
                    mutationReportItems.Add(CreateTesturaMutationReportItem(mutationOperatorType, mutationOperatorBySubCategory.Key, mutationOperatorBySubCategory));
                }
            }

            return mutationReportItems.OrderByDescending(m => m.MutationScore).ToList();
        }

        private TesturaMutationReportItem CreateTesturaMutationReportItem(
            MutationOperators headCategory,
            string subCategory,
            IGrouping<string, MutationDocumentResult> mutationDocumentResults)
        {
            var survived = mutationDocumentResults.Count(m => m.Survived);
            var killed = mutationDocumentResults.Count(m => !m.Survived && m.CompilationResult.IsSuccess);

            return new TesturaMutationReportItem
            {
                HeadCategory = headCategory.ToString(),
                SubCategory = subCategory,
                FailedOnCompilation = mutationDocumentResults.Count(m => !m.CompilationResult.IsSuccess),
                Killed = killed,
                Survived = survived,
                MutationScore = (double)killed / (survived + killed),
            };
        }
    }
}
