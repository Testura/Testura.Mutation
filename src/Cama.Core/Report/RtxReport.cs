using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using Anotar.Log4Net;
using Cama.Core.Models.Mutation;
using ConsoleTables;

namespace Cama.Core.Report
{
    public class RtxReport
    {
        public static void SaveReport(IList<MutationDocumentResult> mutations, string path)
        {
            LogTo.Info("Saving RTX report..");
            var unitTestResults = CreateUnitTestResults(mutations);

            var results = new ResultsType
            {
                Items = new List<UnitTestResultType>(unitTestResults).ToArray(),
            };

            results.ItemsElementName = new ItemsChoiceType3[results.Items.Length];
            for (int n = 0; n < results.Items.Length; n++)
            {
                results.ItemsElementName[n] = ItemsChoiceType3.UnitTestResult;
            }

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
                                passed = mutations.Count(s => !s.Survived && s.CompilerResult.IsSuccess),
                                failed = mutations.Count(s => s.Survived && s.CompilerResult.IsSuccess),
                                completed = mutations.Count(s => s.CompilerResult.IsSuccess),
                                error = mutations.Count(s => !s.CompilerResult.IsSuccess)
                            }
                        }
                    },
                    results
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

        private static UnitTestResultType[] CreateUnitTestResults(IList<MutationDocumentResult> mutations)
        {
            var unitTestResults = new List<UnitTestResultType>();
            foreach (var mutation in mutations)
            {
                string error = string.Empty;

                if (!mutation.CompilerResult.IsSuccess)
                {
                    var errorTable = new ConsoleTable("Description", "File");
                    foreach (var compilerResultError in mutation.CompilerResult.Errors)
                    {
                        errorTable.AddRow(compilerResultError.Message, compilerResultError.Location);
                    }

                    error = $"\n{errorTable.ToStringAlternative()}\n";
                }

                var fileLoadException = mutation.TestResult?.TestResults.FirstOrDefault(t => t.InnerText != null && t.InnerText.Contains("System.IO.FileLoadException : Could not load file or assembly"));
                if (fileLoadException != null)
                {
                    error += $"\nWARNING: It seems like we can't find a file so this result may be invalid: {fileLoadException}";
                }

                var table = new ConsoleTable(" ", " ");
                table.AddRow("Project", mutation.Document.ProjectName);
                table.AddRow("File", mutation.Document.FileName);
                table.AddRow("Where", mutation.Document.MutationInfo.Location.Where);
                table.AddRow("Line", mutation.Document.MutationInfo.Location.Line);
                table.AddRow("Orginal", mutation.Document.MutationInfo.Orginal);
                table.AddRow("Mutation", mutation.Document.MutationInfo.Mutation);
                table.AddRow("Tests run", mutation.TestResult?.TestResults.Count ?? 0);
                table.AddRow("Failed tests", mutation.TestResult?.TestResults.Count(t => !t.IsSuccess) ?? 0);

                unitTestResults.Add(new UnitTestResultType
                {
                    testName = mutation.Document.MutationName,
                    outcome = mutation.Survived ? "Failed" : "Passed",
                    Items = new object[]
                    {
                        new OutputType
                        {
                            StdOut = $"\n{table.ToStringAlternative()}\n",
                            StdErr = error
                        }
                    }
                });
            }

            return unitTestResults.ToArray();
        }
    }
}