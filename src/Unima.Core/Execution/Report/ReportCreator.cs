using System;
using System.Collections.Generic;

namespace Unima.Core.Execution.Report
{
    public abstract class ReportCreator
    {
        protected ReportCreator(string savePath)
        {
            SavePath = savePath;
        }

        protected string SavePath { get; }

        public abstract void SaveReport(IList<MutationDocumentResult> mutations, TimeSpan executionTime);
    }
}
