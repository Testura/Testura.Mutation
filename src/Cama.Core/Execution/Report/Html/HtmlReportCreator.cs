using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Anotar.Log4Net;
using RazorEngine;
using RazorEngine.Templating;

namespace Cama.Core.Execution.Report.Html
{
    public class HtmlReportCreator : ReportCreator
    {
        public HtmlReportCreator(string savePath)
            : base(savePath)
        {
        }

        private string TemplatePath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Execution", "Report", "Html", "ReportTemplate.cshtml");

        public override void SaveReport(IList<MutationDocumentResult> mutations, TimeSpan exectutionTime)
        {
            LogTo.Info("Saving HTML report..");

            if (!mutations.Any(m => m.Survived))
            {
                LogTo.Info("No mutations to report.");
                return;
            }

            try
            {
                var text = File.ReadAllText(TemplatePath);
                var renderedText = Engine.Razor.RunCompile(text, "report", null, mutations.Where(m => m.Survived));

                File.WriteAllText(SavePath, renderedText);
            }
            catch (Exception ex)
            {
                LogTo.ErrorException("Failed to save html report", ex);
                throw;
            }

            LogTo.Info("Html report saved successfully.");
        }
    }
}
