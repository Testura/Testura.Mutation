using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using log4net;

namespace Testura.Mutation.Core.Execution.Report.Markdown
{
    public class MarkdownReportCreator : ReportCreator
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MarkdownReportCreator));

        public MarkdownReportCreator(string path)
            : base(path)
        {
        }

        public override void SaveReport(IList<MutationDocumentResult> mutations, TimeSpan exectutionTime)
        {
            Log.Info("Saving markdown report..");

            if (!mutations.Any(m => m.Survived))
            {
                Log.Info("No mutations to report.");
                return;
            }

            var markdown = CreateMarkdown(mutations);

            try
            {
                File.WriteAllText(SavePath, markdown.ToString());
            }
            catch (IOException ex)
            {
                Log.Error("Failed to save markdown report", ex);
                throw;
            }

            Log.Info("Markdown report saved successfully.");
        }

        private static StringBuilder CreateMarkdown(IList<MutationDocumentResult> mutations)
        {
            var surivedMutations = mutations.Where(m => m.Survived);
            var markdown = new StringBuilder();

            markdown.Append("File | Where | Line | Orginal | Mutation \n");
            markdown.Append("--- | --- | --- | --- | --- \n");
            foreach (var mutation in surivedMutations)
            {
                markdown.Append($"| {mutation.FileName} |" +
                                $" {mutation.Location.Where} |" +
                                $" {mutation.Location.Line} |" +
                                $" ``` {mutation.Orginal} ``` |" +
                                $" ``` {mutation.Mutation} ``` |\n");
            }

            return markdown;
        }
    }
}
