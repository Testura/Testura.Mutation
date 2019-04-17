using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Cama.Core.Exceptions;
using Cama.Core.Execution;
using Cama.Core.Execution.Compilation;
using Cama.Core.Execution.Result;
using Cama.Core.Execution.Runners;
using Cama.Core.Solution;
using ConsoleTables;
using Microsoft.CodeAnalysis.MSBuild;
using Newtonsoft.Json.Linq;

namespace Cama.Core.Baseline
{
    public class BaselineCreator
    {
        private readonly IProjectCompiler _projectCompiler;
        private readonly TestRunnerDependencyFilesHandler _testRunnerDependencyFilesHandler;
        private ITestRunnerClient _testRunnerClient;

        public BaselineCreator(
            IProjectCompiler projectCompiler,
            ITestRunnerClient testRunnerClient,
            TestRunnerDependencyFilesHandler testRunnerDependencyFilesHandler)
        {
            _projectCompiler = projectCompiler;
            _testRunnerClient = testRunnerClient;
            _testRunnerDependencyFilesHandler = testRunnerDependencyFilesHandler;
        }

        private string BaselineDirectoryPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestRun", "Baseline");

        public async Task<IList<BaselineInfo>> CreateBaselineAsync(CamaConfig config)
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                LogTo.Info("Opening solution..");
                var solution = await workspace.OpenSolutionAsync(config.SolutionPath);
                return await CreateBaselineAsync(config, solution);
            }
        }

        public async Task<IList<BaselineInfo>> CreateBaselineAsync(CamaConfig config, Microsoft.CodeAnalysis.Solution solution)
        {
            LogTo.Info("Creating baseline and verifying solution/tests..");

            if (Directory.Exists(BaselineDirectoryPath))
            {
                Directory.Delete(BaselineDirectoryPath, true);
            }

            Directory.CreateDirectory(BaselineDirectoryPath);

            foreach (var mutationProject in config.MutationProjects)
            {
                var project = solution.Projects.FirstOrDefault(p => p.Name == mutationProject.Name);
                var result = await _projectCompiler.CompileAsync(BaselineDirectoryPath, project);

                if (!result.IsSuccess)
                {
                    throw new BaselineException(
                        "Failed to compile base line.",
                        new CompilationException(result.Errors.Select(e => e.Message)));
                }
            }

            var baselineInfos = new List<BaselineInfo>();

            foreach (var solutionProjectInfo in config.TestProjects)
            {
                var result = await RunTestAsync(config.TestRunner, solutionProjectInfo, config.MaxTestTimeMin);

                LogBaselineSummary(baselineInfos);

                if (!result.IsSuccess)
                {
                    var failedTests = result.TestResults.Where(t => !t.IsSuccess);
                    LogTo.Error("Unit tests failed with base line");
                    foreach (var failedTest in failedTests)
                    {
                        LogTo.Error(JObject.FromObject(new { TestName = failedTest.Name, Message = failedTest.InnerText }).ToString());
                    }

                    throw new BaselineException("Failed to run all unit tests with baseline which make mutation testing impossible. See log for more details.");
                }

                LogTo.Info($"..done ({result.TestResults.Count(t => t.IsSuccess)} tests passed).");
                baselineInfos.Add(new BaselineInfo(solutionProjectInfo.Name, result.ExecutionTime));
            }

            LogTo.Info("Baseline completed.");
            return baselineInfos;
        }

        private async Task<TestSuiteResult> RunTestAsync(
            string testRunnerName,
            SolutionProjectInfo testProject,
            int maxTestTimeMin)
        {
            LogTo.Info($"Starting to run tests in {testProject.OutputFileName}");
            var testDirectoryPath = Path.Combine(BaselineDirectoryPath, Guid.NewGuid().ToString());
            var testDllPath = Path.Combine(testDirectoryPath, testProject.OutputFileName);
            Directory.CreateDirectory(testDirectoryPath);

            // Copy all files from the test directory to our own mutation test directory
            _testRunnerDependencyFilesHandler.CopyDependencies(testProject.OutputDirectoryPath, testDirectoryPath);

            foreach (var file in Directory.EnumerateFiles(BaselineDirectoryPath))
            {
                File.Copy(file, Path.Combine(testDirectoryPath, Path.GetFileName(file)), true);
            }

            return await _testRunnerClient.RunTestsAsync(testRunnerName, testDllPath, TimeSpan.FromMinutes(maxTestTimeMin));
        }

        private void LogBaselineSummary(IList<BaselineInfo> baselineInfos)
        {
            var table = new ConsoleTable("Project", "Execution time");
            foreach (var configBaselineInfo in baselineInfos)
            {
                table.AddRow(configBaselineInfo.TestProjectName, configBaselineInfo.ExecutionTime);
            }

            LogTo.Info($"\n{table.ToStringAlternative()}");
        }
    }
}
