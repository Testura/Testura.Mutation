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
    public class TrxReport
    {
        public static void SaveReport(IList<MutationDocumentResult> mutations, string path)
        {
            LogTo.Info("Saving TRX report..");

            if (!mutations.Any())
            {
                LogTo.Info("No mutations to report.");
                return;
            }

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
                                passed = mutations.Count(s => !s.Survived && s.CompilerResult.IsSuccess),
                                failed = mutations.Count(s => s.Survived && s.CompilerResult.IsSuccess),
                                completed = mutations.Count(s => s.CompilerResult.IsSuccess),
                                error = mutations.Count(s => !s.CompilerResult.IsSuccess)
                            }
                        }
                    },
                    results,
                    CreateTestDefinitions(results),
                    CreateTestEntries(results),
                    new TestRunTypeTestLists()
                    {
                        TestList = new TestListType[] { new TestListType() { id = ((UnitTestResultType)results.Items[0]).testListId, name = "All Loaded Results" } }
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

                File.WriteAllText(path, xml);
            }

            LogTo.Info("..TRX report saved.");
        }

        private static TestEntriesType1 CreateTestEntries(ResultsType results)
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

        private static TestDefinitionType CreateTestDefinitions(ResultsType results)
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

        private static UnitTestResultType[] CreateUnitTestResults(IList<MutationDocumentResult> mutations)
        {
            var unitTestResults = new List<UnitTestResultType>();
            var testListId = Guid.NewGuid().ToString();

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
                    testType = "13cdc9d9-ddb5-4fa4-a97d-d965ccfc6d4b",
                    testName = mutation.Document.MutationName,
                    outcome = !mutation.CompilerResult.IsSuccess ? "Ignored" : mutation.Survived ? "Failed" : "Passed",
                    testId = Guid.NewGuid().ToString(),
                    testListId = testListId,
                    executionId = Guid.NewGuid().ToString(),
                    computerName = "mutation",
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