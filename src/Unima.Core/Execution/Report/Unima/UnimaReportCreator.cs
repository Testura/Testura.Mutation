using System;
using System.Collections.Generic;
using System.IO;
using Anotar.Log4Net;
using Newtonsoft.Json;

namespace Unima.Core.Execution.Report.Unima
{
    public class UnimaReportCreator : ReportCreator
    {
        public UnimaReportCreator(string savePath)
            : base(savePath)
        {
        }

        public override void SaveReport(IList<MutationDocumentResult> mutations, TimeSpan executionTime)
        {
            LogTo.Info("Saving unima report..");

            var unimaReport = new UnimaReport(mutations, executionTime);
            File.WriteAllText(SavePath, JsonConvert.SerializeObject(unimaReport));

            LogTo.Info("Unima report saved successfully.");
        }
    }
}
