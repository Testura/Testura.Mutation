using System.Collections.Generic;
using Cama.Core;
using Microsoft.Win32;

namespace Cama.Services
{
    public class SaveReportService
    {
        public void SaveReport(IList<MutationDocumentResult> mutations)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Html file (*.html)|*.html";
            if (saveFileDialog.ShowDialog() == true)
            {
                /*
                new HtmlReportCreator().SaveReport(saveFileDialog.FileName, mutations);
                */
            }
        }
    }
}
