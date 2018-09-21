using System.Collections.Generic;
using Cama.Core.Mutation.Models;

namespace Cama.Core.Report
{
    public interface IReportCreator
    {
        void SaveReport(string savePath, IList<MutationDocumentResult> mutations);
    }
}
