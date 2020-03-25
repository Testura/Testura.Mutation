using System;
using System.Collections.Generic;
using System.IO;
using log4net;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

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
            using (var file = File.CreateText(SavePath))
            {
                var serializer = new JsonSerializer();
                serializer.Converters.Add(new StringEnumConverter());
                serializer.Serialize(file, mutationReport);
            }

            Log.Info("Testura.Mutation report saved successfully.");
        }
    }
}
