using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Cama.Core.Models.Mutation;
using RazorEngine;
using RazorEngine.Templating;

namespace Cama.Core.Report
{
    public static class HtmlReport
    {
        public static void SaveReport(IList<MutationDocumentResult> survivedMutations, string path)
        {
            var text = File.ReadAllText(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "Report", "ReportTemplate.cshtml"));
            var renderedText = Engine.Razor.RunCompile(text, "report", null, survivedMutations);

            File.WriteAllText(path, renderedText);
        }
    }
}
