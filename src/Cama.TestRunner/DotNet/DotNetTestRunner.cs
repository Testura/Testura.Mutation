using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Cama.Core.Execution.Report.Trx;
using Cama.Core.Execution.Result;
using Cama.Core.Execution.Runners;
using Medallion.Shell;

namespace Cama.TestRunner.DotNet
{
    public class DotNetTestRunner : ITestRunner
    {
        private const string ResultName = "result.trx";

        public Task<TestSuiteResult> RunTestsAsync(string dllPath)
        {
            var directoryPath = Path.GetDirectoryName(dllPath);

            return Task.Run(() =>
            {
                using (var command = Command.Run(
                    "dotnet.exe",
                    new[] { "vstest", dllPath, $"--logger:trx;LogFileName={ResultName}", $"--ResultsDirectory:{directoryPath}" },
                    o =>
                    {
                        o.StartInfo(si =>
                        {
                            si.CreateNoWindow = true;
                            si.UseShellExecute = false;
                            si.RedirectStandardError = true;
                            si.RedirectStandardInput = true;
                            si.RedirectStandardOutput = true;
                        });
                        o.DisposeOnExit();
                    }))
                {
                    var error = command.StandardError.ReadToEnd();

                    if (!command.Result.Success && !error.ToLower().Contains("test run failed"))
                    {
                        return TestSuiteResult.Error(error, TimeSpan.Zero);
                    }

                    return CreateResult(Path.GetFileNameWithoutExtension(dllPath), directoryPath);
                }
            });
        }

        private TestSuiteResult CreateResult(string name, string directoryPath)
        {
            var serializer = new XmlSerializer(typeof(TestRunType));

            using (var fileStream = new FileStream(Path.Combine(directoryPath, ResultName), FileMode.Open))
            {
                var testRun = (TestRunType)serializer.Deserialize(fileStream);
                var time = (TestRunTypeTimes)testRun.Items[0];

                var results = testRun.Items[2] as ResultsType;
                var tests = results.Items.Select(i => i as UnitTestResultType);

                var testDefinitions = testRun.Items[3] as TestDefinitionType;
                var testDefinitionItems = testDefinitions.Items.Select(i => i as UnitTestType);

                var testResults = new List<TestResult>();
                foreach (var unitTestResultType in tests)
                {
                    testResults.Add(new TestResult
                    {
                        FullName = $"{testDefinitionItems.First(t => t.id == unitTestResultType.testId).TestMethod.className}.{unitTestResultType.testName}",
                        IsSuccess = unitTestResultType.outcome.Equals("passed", StringComparison.InvariantCultureIgnoreCase),
                        Name = unitTestResultType.testName
                    });
                }

                return new TestSuiteResult
                {
                    ExecutionTime = DateTime.Parse(time.finish) - DateTime.Parse(time.start),
                    IsSuccess = testResults.All(t => t.IsSuccess),
                    Name = name,
                    TestResults = testResults
                };
            }
        }
    }
}
