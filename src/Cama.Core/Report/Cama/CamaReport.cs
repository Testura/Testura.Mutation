using System.Collections.Generic;
using System.Linq;
using Cama.Core.Mutation.Models;

namespace Cama.Core.Report.Cama
{
    public class CamaReport
    {
        public CamaReport()
        {
        }

        public CamaReport(IList<MutationDocumentResult> mutations)
        {
            TotalNumberOfMutations = mutations.Count;
            NumberOfSurvivedMutations = mutations.Count(m => m.Survived);
            Mutations = mutations;
        }

        public int TotalNumberOfMutations { get; set; }

        public int NumberOfSurvivedMutations { get; set; }

        public IEnumerable<MutationDocumentResult> Mutations { get; set; }
    }
}
