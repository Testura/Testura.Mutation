using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Log4Net;
using ConsoleTables;
using Newtonsoft.Json.Linq;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Exceptions;
using Testura.Mutation.Core.Execution;
using Testura.Mutation.Core.Execution.Compilation;
using Testura.Mutation.Core.Execution.Result;
using Testura.Mutation.Core.Execution.Runners;
using Testura.Mutation.Core.Solution;

namespace Testura.Mutation.Core.Baseline
{
    public class BaselineCreator
    {
        private readonly IProjectCompiler _projectCompiler;
        private readonly TestRunnerDependencyFilesHandler _testRunnerDependencyFilesHandler;
        private readonly ISolutionOpener _solutionOpener;
        private ITestRunnerClient _testRunnerClient;

        public BaselineCreator(
            IProjectCompiler projectCompiler,
            ITestRunnerClient testRunnerClient,
            ISolutionOpener solutionOpener,
            TestRunnerDependencyFilesHandler testRunnerDependencyFilesHandler)
        {
            _projectCompiler = projectCompiler;
            _testRunnerClient = testRunnerClient;
            _solutionOpener = solutionOpener;
            _testRunnerDependencyFilesHandler = testRunnerDependencyFilesHandler;
        }

        private string BaselineDirectoryPath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestRun", "Baseline");

        public async Task<IList<BaselineInfo>> CreateBaselineAsync(MutationConfig config, CancellationToken cancellationToken = default(CancellationToken))
        {
            LogTo.Info("Opening solution..");
            var solution = await _solutionOpener.GetSolutionAsync(config);
            return await CreateBaselineAsync(config, solution, cancellationToken);
        }

        public async Task<IList<BaselineInfo>> CreateBaselineAsync(MutationConfig config, Microsoft.CodeAnalysis.Solution solution, CancellationToken cancellationToken = default(CancellationToken))
        {
            LogTo.Info("Creating baseline and verifying solution/tests..");

            DeleteBaselineDirectory();

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                Directory.CreateDirectory(BaselineDirectoryPath);

                foreach (var mutationProject in config.MutationProjects)
                {
                    var project = solution.Projects.FirstOrDefault(p => p.Name == mutationProject.Name);
                    var result = await _projectCompiler.CompileAsync(BaselineDirectoryPath, project);

                    if (!result.IsSuccess)
                    {
                        foreach (var compilationError in result.Errors)
                        {
                            LogTo.Error($"{{ Error = \"{compilationError.Message}\", Location = \"{compilationError.Location}\"");
                        }

                        throw new BaselineException(
                            "Failed to compile base line.",
                            new CompilationException(result.Errors.Select(e => e.Message)));
                    }
                }

                var baselineInfos = new List<BaselineInfo>();

                foreach (var testProject in config.TestProjects)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var result = await RunTestAsync(testProject, config.DotNetPath, config.MaxTestTimeMin, cancellationToken);

                    cancellationToken.ThrowIfCancellationRequested();

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

                LogBaselineSummary(baselineInfos);

                LogTo.Info("Baseline completed.");
                return baselineInfos;
            }
            catch (OperationCanceledException)
            {
                LogTo.Info("Cancellation request when running baseline.");
                return new List<BaselineInfo>();
            }
            finally
            {
                DeleteBaselineDirectory();
            }
        }

        private async Task<TestSuiteResult> RunTestAsync(
            TestProject testProject,
            string dotNetPath,
            int maxTestTimeMin,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            LogTo.Info($"Starting to run tests in {testProject.Project.OutputFileName}");
            var testDirectoryPath = Path.Combine(BaselineDirectoryPath, Guid.NewGuid().ToString());
            var testDllPath = Path.Combine(testDirectoryPath, testProject.Project.OutputFileName);
            Directory.CreateDirectory(testDirectoryPath);

            // Copy all files from the test directory to our own mutation test directory
            _testRunnerDependencyFilesHandler.CopyDependencies(testProject.Project.OutputDirectoryPath, testDirectoryPath);

            foreach (var file in Directory.EnumerateFiles(BaselineDirectoryPath))
            {
                File.Copy(file, Path.Combine(testDirectoryPath, Path.GetFileName(file)), true);
            }

            return await _testRunnerClient.RunTestsAsync(testProject.TestRunner, testDllPath, dotNetPath, TimeSpan.FromMinutes(maxTestTimeMin), cancellationToken);
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

        private void DeleteBaselineDirectory()
        {
            try
            {
                if (Directory.Exists(BaselineDirectoryPath))
                {
                    Directory.Delete(BaselineDirectoryPath, true);
                }
            }
            catch (Exception ex)
            {
                LogTo.Error($"Failed to delete baseline directory: {ex.Message}");
            }
        }
    }
}
