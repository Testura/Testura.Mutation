using System.Collections.Generic;
using Cama.Core.Mutation.Models;
using Cama.Core.Report.Html;
using Microsoft.Win32;

namespace Cama.Module.Mutation.Services
{
    public class SaveReportService
    {
        public void SaveReport(IList<MutationDocumentResult> mutations)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Html file (*.html)|*.html";
            if (saveFileDialog.ShowDialog() == true)
            {
                new HtmlReportCreator().SaveReport(saveFileDialog.FileName, mutations);
            }
        }
    }
}
