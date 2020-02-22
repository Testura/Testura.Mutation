using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using Newtonsoft.Json;

namespace Testura.Mutation.Core.Execution.Report.Testura
{
    public class TesturaMutationReportCreator : ReportCreator
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TesturaMutationReportCreator));

        public TesturaMutationReportCreator(string savePath)
            : base(savePath)
        {
        }

        public override void SaveReport(IList<MutationDocumentResult> mutations, TimeSpan executionTime)
        {
            Log.Info("Saving Testura.Mutation report..");

            var mutationReport = new TesturaMutationReport(mutations, executionTime);
            using (StreamWriter file = File.CreateText(SavePath))
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(file, mutationReport);
            }

            Log.Info("Testura.Mutation report saved successfully.");
        }
    }
}
