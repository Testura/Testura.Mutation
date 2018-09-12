using System.Collections.Generic;
using Cama.Core.Models.Mutation;

namespace Cama.Core.Report
{
    public abstract class ReportCreator
    {
        protected ReportCreator(string savePath)
        {
            SavePath = savePath;
        }

        protected string SavePath { get; }

        public abstract void SaveReport(IList<MutationDocumentResult> mutations);
    }
}
