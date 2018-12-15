using System;
using System.Collections.Generic;
using System.Linq;

namespace Cama.Core.Execution.Report.Cama
{
    public class CamaReport
    {
        public CamaReport()
        {
        }

        public CamaReport(IList<MutationDocumentResult> mutations, TimeSpan executionTime)
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
