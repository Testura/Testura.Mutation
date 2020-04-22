using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Newtonsoft.Json.Linq;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Exceptions;
using Testura.Mutation.Core.Execution;
using Testura.Mutation.Core.Execution.Result;
using Testura.Mutation.Core.Execution.Runners;

namespace Testura.Mutation.Core.Baseline.Handlers
{
    public class BaselineCreatorRunUnitTestsHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(BaselineCreatorRunUnitTestsHandler));

        private readonly ITestRunnerClient _testRunnerClient;
        private readonly TestRunnerDependencyFilesHandler _testRunnerDependencyFilesHandler;

        public BaselineCreatorRunUnitTestsHandler(
            ITestRunnerClient testRunnerClient,
            TestRunnerDependencyFilesHandler testRunnerDependencyFilesHandler)
        {
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
                    Log.Error("Unit tests failed with base line");
                    Log.Error($"Name: {result.Name}");

                    foreach (var failedTest in failedTests)
                    {
                        Log.Error(JObject.FromObject(new { TestName = failedTest.Name, Message = failedTest.InnerText }).ToString());
                    }

                    throw new BaselineException("Failed to run all unit tests with baseline which make mutation testing impossible. See log for more details.");
                }

                Log.Info($"..done ({result.TestResults.Count(t => t.IsSuccess)} tests passed).");
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
            Log.Info($"Starting to run tests in {testProject.Project.OutputFileName} ({testProject.Project.Name})");

            var testDllPath = _testRunnerDependencyFilesHandler.CreateTestDirectoryAndCopyDependencies(baselineDirectoryPath, testProject, baselineDirectoryPath);
            return await _testRunnerClient.RunTestsAsync(testProject.TestRunner, testDllPath, dotNetPath, TimeSpan.FromMinutes(maxTestTimeMin), cancellationToken);
        }
    }
}
