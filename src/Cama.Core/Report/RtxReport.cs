using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Anotar.Log4Net;
using Cama.Core.Models.Mutation;

namespace Cama.Core.Report
{
    public class RtxReport
    {
        public static void SaveReport(IList<MutationDocumentResult> mutations, string path)
        {
            LogTo.Info("Saving RTX report..");
            var unitTestResults = CreateUnitTestResults(mutations);

            var testRunType = new TestRunType
            {
                Items = new object[]
                {
                    new TestRunTypeTimes
                    {
                        creation = DateTime.Now.ToString(),
                        finish = DateTime.Now.ToString(),
                        start = DateTime.Now.ToString()
                    },
                    new TestRunTypeResultSummary
                    {
                        Items = new object[]
                        {
                            new CountersType
                            {
                                passed = mutations.Count(s => !s.Survived),
                                failed = mutations.Count(s => s.Survived && s.CompilerResult.IsSuccess),
                                completed = mutations.Count(s => s.CompilerResult.IsSuccess),
                                error = mutations.Count(s => !s.CompilerResult.IsSuccess)
                            }
                        }
                    },
                    new ResultsType
                    {
                        Items = new List<object>(unitTestResults).ToArray()
                    }
                }
            };

            var ser = new XmlSerializer(typeof(TestRunType));
            using (var writer = new StreamWriter(path))
            {
                ser.Serialize(writer, testRunType);
                writer.Close();
            }

            LogTo.Info("..RTX report saved.");
        }

        private static UnitTestResultType[] CreateUnitTestResults(IList<MutationDocumentResult> survivedMutations)
        {
            var unitTestResults = new List<UnitTestResultType>();
            foreach (var mutation in survivedMutations)
            {
                unitTestResults.Add(new UnitTestResultType
                {
                    testName = $"{mutation.Document.ProjectName}.{mutation.Document.FileName}({mutation.Document.MutationInfo.Location.Where} - {mutation.Document.MutationInfo.Location.Line})",
                    outcome = mutation.Survived ? "Failed" : "Passed",
                    Items = new object[]
                    {
                        new OutputType
                        {
                            StdOut = $@"
                                Project = {mutation.Document.ProjectName}
                                File = {mutation.Document.FileName}
                                Where = {mutation.Document.MutationInfo.Location.Where}
                                Line = {mutation.Document.MutationInfo.Location.Line}
                                Orginal = {mutation.Document.MutationInfo.Orginal}
                                Mutation = {mutation.Document.MutationInfo.Mutation}
                            "
                        }
                    }
                });
            }

            return unitTestResults.ToArray();
        }
    }
}