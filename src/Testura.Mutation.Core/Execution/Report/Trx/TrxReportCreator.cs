using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using log4net;

namespace Testura.Mutation.Core.Execution.Report.Trx
{
    public class TrxReportCreator : ReportCreator
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(TrxReportCreator));

        public TrxReportCreator(string path)
            : base(path)
        {
        }

        public override void SaveReport(IList<MutationDocumentResult> mutations, TimeSpan exectutionTime)
        {
            Log.Info("Saving TRX report..");

            if (!mutations.Any())
            {
                Log.Info("No mutations to report.");
                return;
            }

            try
            {
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
                    id = Guid.NewGuid().ToString(),
                    name = "mutation",
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
                            outcome = mutations.All(m => !m.Survived) ? "Passed" : "Failed",
                            Items = new object[]
                            {
                                new CountersType
                                {
                                    passed = mutations.Count(s => !s.Survived && s.CompilationResult != null && s.CompilationResult.IsSuccess),
                                    failed = mutations.Count(s => s.Survived && s.CompilationResult != null && s.CompilationResult.IsSuccess),
                                    completed = mutations.Count(s => s.CompilationResult != null && s.CompilationResult.IsSuccess),
                                    error = mutations.Count(s => s.CompilationResult == null || !s.CompilationResult.IsSuccess)
                                }
                            }
                        },
                        results,
                        CreateTestDefinitions(results),
                        CreateTestEntries(results),
                        new TestRunTypeTestLists()
                        {
                            TestList = new TestListType[]
                            {
                                new TestListType()
                                {
                                    id = results.Items.Any() ? ((UnitTestResultType)results.Items[0]).testListId : "No result found",
                                    name = "All Loaded Results"
                                }
                            }
                        }
                    }
                };

                var xmlSerializer = new XmlSerializer(typeof(TestRunType));
                using (var textWriter = new StringWriter())
                {
                    xmlSerializer.Serialize(textWriter, testRunType);
                    var xml = textWriter
                        .ToString()
                        .Replace(" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", string.Empty)
                        .Replace(" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", string.Empty)
                        .Replace(" xsi:type=\"xsd:string\"", string.Empty);

                    File.WriteAllText(SavePath, xml);
                }
            }
            catch (Exception ex)
            {
                Log.Error("Failed to save TRX report", ex);
                throw;
            }

            Log.Info("TRX report saved successfully.");
        }

        private TestEntriesType1 CreateTestEntries(ResultsType results)
        {
            var entries = new TestEntryType[results.Items.Length];
            for (int n = 0; n < entries.Length; n++)
            {
                var r = results.Items[n] as UnitTestResultType;
                entries[n] = new TestEntryType
                {
                    executionId = r.executionId,
                    testId = r.testId,
                    testListId = r.testListId
                };
            }

            return new TestEntriesType1
            {
                TestEntry = entries
            };
        }

        private TestDefinitionType CreateTestDefinitions(ResultsType results)
        {
            var unitTestDefinitions = new List<UnitTestType>();
            foreach (var result in results.Items)
            {
                var r = result as UnitTestResultType;

                unitTestDefinitions.Add(new UnitTestType
                {
                    id = r.testId,
                    name = r.testName,
                    Items = new object[]
                    {
                        new BaseTestTypeExecution
                        {
                            id = r.executionId
                        },
                    },
                    TestMethod = new UnitTestTypeTestMethod
                    {
                        codeBase = "...",
                        adapterTypeName = "Microsoft.VisualStudio.TestTools.TestTypes.Unit.UnitTestAdapter",
                        className = r.testName
                    }
                });
            }

            return new TestDefinitionType
            {
                Items = unitTestDefinitions.ToArray(),
                ItemsElementName = unitTestDefinitions.Select(u => ItemsChoiceType4.UnitTest).ToArray()
            };
        }

        private UnitTestResultType[] CreateUnitTestResults(IList<MutationDocumentResult> mutations)
        {
            var unitTestResults = new List<UnitTestResultType>();
            var testListId = Guid.NewGuid().ToString();

            foreach (var mutation in mutations)
            {
                string error = string.Empty;

                if (mutation.CompilationResult == null)
                {
                    continue;
                }

                if (mutation.CompilationResult != null && !mutation.CompilationResult.IsSuccess)
                {
                    var errorBuilder = new StringBuilder();

                    errorBuilder.Append($"+- {"Description",-40} File -+");

                    foreach (var compilerResultError in mutation.CompilationResult.Errors)
                    {
                        errorBuilder.Append($"{compilerResultError.Message,-40} {compilerResultError.Location} \n");
                    }

                    error = $"\n{errorBuilder}\n";
                }

                var fileLoadException = mutation.FailedTests.FirstOrDefault(t => t.InnerText != null && t.InnerText.Contains("System.IO.FileLoadException : Could not load file or assembly"));
                if (fileLoadException != null)
                {
                    error += $"\nWARNING: It seems like we can't find a file so this result may be invalid: {fileLoadException}";
                }

                var infoBuilder = new StringBuilder();

                infoBuilder.Append($"{"Project",-40} {mutation.ProjectName} \n");
                infoBuilder.Append($"{"File",-40} {mutation.FileName} \n");
                infoBuilder.Append($"{"Where",-40} {mutation.Location.Where} \n");
                infoBuilder.Append($"{"Line",-40} {mutation.Location.Line} \n");
                infoBuilder.Append($"{"Orginal",-40} {mutation.Orginal} \n");
                infoBuilder.Append($"{"Mutation",-40} {mutation.Mutation} \n");
                infoBuilder.Append($"{"Tests run",-40} {mutation.TestsRunCount} \n");
                infoBuilder.Append($"{"Failed tests",-40} {mutation.FailedTests.Count} \n");

                unitTestResults.Add(new UnitTestResultType
                {
                    testType = "13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b",
                    testName = mutation.MutationName,
                    outcome = !mutation.CompilationResult.IsSuccess ? "Ignored" : mutation.Survived ? "Failed" : "Passed",
                    testId = Guid.NewGuid().ToString(),
                    testListId = testListId,
                    executionId = Guid.NewGuid().ToString(),
                    computerName = "mutation",
                    Items = new object[]
                    {
                        new OutputType
                        {
                            StdOut = $"\n{infoBuilder}\n",
                            StdErr = error
                        }
                    }
                });
            }

            return unitTestResults.ToArray();
        }
    }
}