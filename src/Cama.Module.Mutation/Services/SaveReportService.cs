using System.Collections.Generic;
using Cama.Core.Models.Mutation;
using Cama.Core.Report;
using Microsoft.Win32;

namespace Cama.Module.Mutation.Services
{
    public class SaveReportService
    {
        public void SaveReport(IList<MutationDocumentResult> survivedMutations)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Html file (*.html)|*.html";
            if (saveFileDialog.ShowDialog() == true)
            {
                HtmlReport.SaveReport(survivedMutations, saveFileDialog.FileName);
            }
        }
    }
}
