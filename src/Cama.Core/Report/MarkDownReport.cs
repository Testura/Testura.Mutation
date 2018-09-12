using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Anotar.Log4Net;
using Cama.Core.Models.Mutation;

namespace Cama.Core.Report
{
    public static class MarkdownReport
    {
        public static void SaveReport(IList<MutationDocumentResult> mutations, string path)
        {
            LogTo.Info("Saving markdown report..");

            if (!mutations.Any())
            {
                LogTo.Info("No mutations to report.");
                return;
            }

            var surivedMutations = mutations.Where(m => m.Survived);
            var markdown = new StringBuilder();

            markdown.Append("File | Where | Line | Orginal | Mutation \n");
            markdown.Append("--- | --- | --- | --- | --- \n");
            foreach (var mutation in surivedMutations)
            {
                markdown.Append($"{mutation.Document.FileName} |" +
                                $" {mutation.Document.MutationInfo.Location.Where} |" +
                                $" {mutation.Document.MutationInfo.Location.Line} |" +
                                $" `{mutation.Document.MutationInfo.Orginal}` |" +
                                $" `{mutation.Document.MutationInfo.Mutation}`\n");
            }

            File.WriteAllText(path, markdown.ToString());

            LogTo.Info("Saving done.");
        }
    }
}
