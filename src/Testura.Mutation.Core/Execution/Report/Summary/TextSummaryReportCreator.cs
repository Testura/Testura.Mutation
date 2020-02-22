using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using log4net;

namespace Testura.Mutation.Core.Execution.Report.Summary
{
    public class TextSummaryReportCreator : ReportCreator
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TextSummaryReportCreator));

        public TextSummaryReportCreator(string savePath)
            : base(savePath)
        {
        }

        public override void SaveReport(IList<MutationDocumentResult> mutations, TimeSpan executionTime)
        {
            if (!mutations.Any())
            {
                Log.Info("No mutations to report.");
                return;
            }

            var survived = mutations.Count(r => r.Survived && (r.CompilationResult != null && r.CompilationResult.IsSuccess));
            var killed = mutations.Count(r => !r.Survived && (r.CompilationResult != null && r.CompilationResult.IsSuccess));
            var compileErrors = mutations.Count(r => r.CompilationResult != null && !r.CompilationResult.IsSuccess);
            var unknownErrors = mutations.Count(r => r.UnexpectedError != null);

            var lines = new[]
            {
                $"Total: {mutations.Count}",
                $"Survived: {survived}",
                $"Killed: {killed}",
                $"Compile errors: {compileErrors}",
                $"Unknown errors: {unknownErrors}",
                $"Execution time: {executionTime}"
            };

            File.WriteAllLines(SavePath, lines);
        }
    }
}
