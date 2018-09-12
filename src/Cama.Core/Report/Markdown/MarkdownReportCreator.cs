using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Anotar.Log4Net;
using Cama.Core.Models.Mutation;

namespace Cama.Core.Report.Markdown
{
    public class MarkdownReportCreator : ReportCreator
    {
        public MarkdownReportCreator(string savePath)
            : base(savePath)
        {
        }

        public override void SaveReport(IList<MutationDocumentResult> mutations)
        {
            LogTo.Info("Saving markdown report..");

            if (!mutations.Any(m => m.Survived))
            {
                LogTo.Info("No mutations to report.");
                return;
            }

            var markdown = CreateMarkdown(mutations);

            try
            {
                File.WriteAllText(SavePath, markdown.ToString());
            }
            catch (IOException ex)
            {
                LogTo.ErrorException("Failed to save markdown report", ex);
                throw;
            }

            LogTo.Info("Markdown report saved successfully.");
        }

        private static StringBuilder CreateMarkdown(IList<MutationDocumentResult> mutations)
        {
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

            return markdown;
        }
    }
}
