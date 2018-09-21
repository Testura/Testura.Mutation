using System.Collections.Generic;
using System.IO;
using Anotar.Log4Net;
using Cama.Core.Mutation.Models;
using Newtonsoft.Json;

namespace Cama.Core.Report.Cama
{
    public class CamaReportCreator : IReportCreator
    {
        public void SaveReport(string savePath, IList<MutationDocumentResult> mutations)
        {
            LogTo.Info("Saving cama report..");

            var camaReport = new CamaReport(mutations);
            File.WriteAllText(savePath, JsonConvert.SerializeObject(camaReport));

            LogTo.Info("Cama report saved successfully.");
        }
    }
}
