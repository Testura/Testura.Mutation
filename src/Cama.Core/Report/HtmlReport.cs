using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Anotar.Log4Net;
using Cama.Core.Models.Mutation;
using RazorEngine;
using RazorEngine.Templating;

namespace Cama.Core.Report
{
    public static class HtmlReport
    {
        public static void SaveReport(IList<MutationDocumentResult> mutations, string path)
        {
            LogTo.Info("Saving HTML report..");

            if (!mutations.Any(m => m.Survived))
            {
                LogTo.Info("No mutations to report.");
                return;
            }

            var text = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Report", "ReportTemplate.cshtml"));
            var renderedText = Engine.Razor.RunCompile(text, "report", null, mutations.Where(m => m.Survived));

            File.WriteAllText(path, renderedText);

            LogTo.Info("Saving done.");
        }
    }
}
