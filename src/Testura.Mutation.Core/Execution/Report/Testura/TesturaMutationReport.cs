using System;
using System.Collections.Generic;
using System.Linq;

namespace Testura.Mutation.Core.Execution.Report.Testura
{
    public class TesturaMutationReport
    {
        public TesturaMutationReport()
        {
        }

        public TesturaMutationReport(IList<MutationDocumentResult> mutations, TimeSpan executionTime)
        {
            TotalNumberOfMutations = mutations.Count;
            NumberOfSurvivedMutations = mutations.Count(m => m.Survived);
            NumberOfKilledMutations = mutations.Count(r => !r.Survived && (r.CompilationResult != null && r.CompilationResult.IsSuccess));
            Mutations = mutations;
            ExecutionTime = executionTime;
        }

        public int NumberOfKilledMutations { get; set; }

        public TimeSpan ExecutionTime { get; set; }

        public int TotalNumberOfMutations { get; set; }

        public int NumberOfSurvivedMutations { get; set; }

        public IEnumerable<MutationDocumentResult> Mutations { get; set; }
    }
}
