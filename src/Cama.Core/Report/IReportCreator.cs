using System.Collections.Generic;
using Cama.Core.Models.Mutation;

namespace Cama.Core.Report
{
    public interface IReportCreator
    {
        void SaveReport(string savePath, IList<MutationDocumentResult> mutations);
    }
}
