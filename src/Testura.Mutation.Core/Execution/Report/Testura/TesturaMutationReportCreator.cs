using System;
using System.Collections.Generic;
using System.IO;
using Anotar.Log4Net;
using Newtonsoft.Json;

namespace Testura.Mutation.Core.Execution.Report.Testura
{
    public class TesturaMutationReportCreator : ReportCreator
    {
        public TesturaMutationReportCreator(string savePath)
            : base(savePath)
        {
        }

        public override void SaveReport(IList<MutationDocumentResult> mutations, TimeSpan executionTime)
        {
            LogTo.Info("Saving Testura.Mutation report..");

            var mutationReport = new TesturaMutationReport(mutations, executionTime);
            using (StreamWriter file = File.CreateText(SavePath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, mutationReport);
            }

            LogTo.Info("Testura.Mutation report saved successfully.");
        }
    }
}
