using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Medallion.Shell;
using Medallion.Shell.Streams;
using Testura.Mutation.Core.Execution.Report.Trx;
using Testura.Mutation.Core.Execution.Result;
using Testura.Mutation.Core.Execution.Runners;

namespace Testura.Mutation.TestRunner.Console.DotNet
{
    public class DotNetTestRunner : ITestRunner
    {
        private readonly string _resultId;
        private readonly string _dotNetPath;
        private readonly TimeSpan _maxTime;

        public DotNetTestRunner(string dotNetPath, TimeSpan maxTime)
        {
            _dotNetPath = dotNetPath;
            _maxTime = maxTime;
            _resultId = Guid.NewGuid().ToString();
        }

        public Task<TestSuiteResult> RunTestsAsync(string dllPath)
        {
            return RunTestWithRetries(dllPath, 2);
        }

        public Task<TestSuiteResult> RunTestWithRetries(string dllPath, int retries)
        {
            var directoryPath = Path.GetDirectoryName(dllPath);

            return Task.Run(async () =>
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
                    ReadToEnd(command.StandardError, out var error);
                    ReadToEnd(command.StandardOutput, out var message);

                    var resultFile = GetResultFile(directoryPath);
                    var success = !string.IsNullOrEmpty(resultFile);

                    if (!success)
                    {
                        command.Kill();

                        // Let's do a retry if we get error from both message and error stream.
                        if (error.Contains("Error reading from stream") && message.Contains("Error reading from stream") && retries > 0)
                        {
                            return await RunTestWithRetries(dllPath, retries - 1);
                        }

                        return TestSuiteResult.Error($"{{ Message = \"{message}\", Error = \"{error}\"", TimeSpan.Zero);
                    }

                    try
                    {
                        return CreateResult(Path.GetFileNameWithoutExtension(dllPath), directoryPath, message);
                    }
                    catch (Exception ex)
                    {
                        return TestSuiteResult.Error($"Unexpected error when parsing test result. {{ Exception: \"{ex.Message}\" Message = \"{message}\", Error = \"{error}\"", TimeSpan.Zero);
                    }
                }
            });
        }

        private TestSuiteResult CreateResult(string name, string directoryPath, string message)
        {
            var serializer = new XmlSerializer(typeof(TestRunType));
            var resultPath = Directory.GetFiles(directoryPath).FirstOrDefault(f => f.Contains(_resultId));

            using (var fileStream = new FileStream(resultPath, FileMode.Open))
            {
                var testRun = (TestRunType)serializer.Deserialize(fileStream);
                var time = (TestRunTypeTimes)testRun.Items[0];

                var results = testRun.Items[2] as ResultsType;

                // If the path something probably have gone bad and we got it on standard output.
                if (results == null)
                {
                    return TestSuiteResult.Error(message, TimeSpan.Zero);
                }

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
                        Name = unitTestResultType.testName,
                        InnerText = TryGetMessage(unitTestResultType)
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

        private string TryGetMessage(UnitTestResultType unitTestResultType)
        {
            if (unitTestResultType.Items == null || !unitTestResultType.Items.Any())
            {
                return string.Empty;
            }

            var item = unitTestResultType.Items.First() as OutputType;
            var message = item?.ErrorInfo?.Message as XmlNode[];

            if (message == null || !message.Any())
            {
                return string.Empty;
            }

            return message[0].Value ?? string.Empty;
        }

        private bool ReadToEnd(ProcessStreamReader processStream, out string message)
        {
            var readStreamTask = Task.Run(
                () =>
                {
                    var streamMessage = string.Empty;

                    while (processStream.Peek() != -1)
                    {
                        streamMessage += processStream.ReadLine();
                    }

                    return streamMessage;
                });

            var successful = readStreamTask.Wait(_maxTime);

            message = successful ? readStreamTask.Result : "Stuck when reading from stream!";
            return successful;
        }

        private string GetDotNetExe()
        {
            return string.IsNullOrEmpty(_dotNetPath) ? "dotnet.exe" : _dotNetPath;
        }

        private string GetResultFile(string directoryPath)
        {
            return Directory.GetFiles(directoryPath).FirstOrDefault(f => f.Contains(_resultId));
        }
    }
}
