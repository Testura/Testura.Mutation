using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Newtonsoft.Json.Linq;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Exceptions;
using Testura.Mutation.Core.Execution;
using Testura.Mutation.Core.Execution.Result;
using Testura.Mutation.Core.Execution.Runners;
using Testura.Mutation.Core.Util.FileSystem;

namespace Testura.Mutation.Core.Baseline.Handlers
{
    public class BaselineCreatorRunUnitTestsHandler
    {
        private readonly IDirectoryHandler _directoryHandler;
        private readonly ITestRunnerClient _testRunnerClient;
        private readonly TestRunnerDependencyFilesHandler _testRunnerDependencyFilesHandler;

        public BaselineCreatorRunUnitTestsHandler(
            IDirectoryHandler directoryHandler,
            ITestRunnerClient testRunnerClient,
            TestRunnerDependencyFilesHandler testRunnerDependencyFilesHandler)
        {
            _directoryHandler = directoryHandler;
            _testRunnerClient = testRunnerClient;
            _testRunnerDependencyFilesHandler = testRunnerDependencyFilesHandler;
        }

        public async Task<IList<BaselineInfo>> RunUnitTests(MutationConfig config, string baselineDirectoryPath, CancellationToken cancellationToken = default(CancellationToken))
        {
            var baselineInfos = new List<BaselineInfo>();

            foreach (var testProject in config.TestProjects)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var result = await RunTestAsync(testProject, config.DotNetPath, config.MaxTestTimeMin, baselineDirectoryPath, cancellationToken);

                if (!result.IsSuccess)
                {
                    var failedTests = result.TestResults.Where(t => !t.IsSuccess);
                    LogTo.Error("Unit tests failed with base line");
                    LogTo.Error($"Name: {result.Name}");

                    foreach (var failedTest in failedTests)
                    {
                        LogTo.Error(JObject.FromObject(new { TestName = failedTest.Name, Message = failedTest.InnerText }).ToString());
                    }

                    throw new BaselineException("Failed to run all unit tests with baseline which make mutation testing impossible. See log for more details.");
                }

                LogTo.Info($"..done ({result.TestResults.Count(t => t.IsSuccess)} tests passed).");
                baselineInfos.Add(new BaselineInfo(testProject.Project.Name, result.ExecutionTime));
            }

            return baselineInfos;
        }

        private async Task<TestSuiteResult> RunTestAsync(
            TestProject testProject,
            string dotNetPath,
            int maxTestTimeMin,
            string baselineDirectoryPath,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            LogTo.Info($"Starting to run tests in {testProject.Project.OutputFileName}");
            var testDirectoryPath = Path.Combine(baselineDirectoryPath, Guid.NewGuid().ToString());
            var testDllPath = Path.Combine(testDirectoryPath, testProject.Project.OutputFileName);

            _directoryHandler.CreateDirectory(testDirectoryPath);

            // Copy all files from the test directory to our own mutation test directory
            _testRunnerDependencyFilesHandler.CopyDependencies(testProject.Project.OutputDirectoryPath, testDirectoryPath);

            foreach (var file in Directory.EnumerateFiles(baselineDirectoryPath))
            {
                File.Copy(file, Path.Combine(testDirectoryPath, Path.GetFileName(file)), true);
            }

            return await _testRunnerClient.RunTestsAsync(testProject.TestRunner, testDllPath, dotNetPath, TimeSpan.FromMinutes(maxTestTimeMin), cancellationToken);
        }
    }
}
