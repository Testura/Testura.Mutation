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
        private readonly string _resultId;
        private readonly string _dotNetPath;

        public DotNetTestRunner(string dotNetPath)
        {
            _dotNetPath = dotNetPath;
            _resultId = Guid.NewGuid().ToString();
        }

        public Task<TestSuiteResult> RunTestsAsync(string dllPath)
        {
            var directoryPath = Path.GetDirectoryName(dllPath);

            return Task.Run(() =>
            {
                using (var command = Command.Run(
                     GetDotNetExe(),
                    new[] { "vstest", dllPath, $"--logger:trx;LogFileName={_resultId}", $"--ResultsDirectory:{directoryPath}" },
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
            var resultPath = Directory.GetFiles(directoryPath).First(f => f.Contains(_resultId));

            using (var fileStream = new FileStream(resultPath, FileMode.Open))
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
                        IsSuccess = unitTestResultType.outcome.Equals("passed", StringComparison.InvariantCultureIgnoreCase) || unitTestResultType.outcome.Equals("NotExecuted", StringComparison.InvariantCultureIgnoreCase),
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

        private string GetDotNetExe()
        {
            return string.IsNullOrEmpty(_dotNetPath) ? "dotnet.exe" : _dotNetPath;
        }
    }
}
