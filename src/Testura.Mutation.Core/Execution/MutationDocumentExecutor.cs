using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Exceptions;
using Testura.Mutation.Core.Execution.Compilation;
using Testura.Mutation.Core.Execution.Result;
using Testura.Mutation.Core.Execution.Runners;
using Testura.Mutation.Core.Extensions;

namespace Testura.Mutation.Core.Execution
{
    public class MutationDocumentExecutor
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(MutationDocumentExecutor));

        private readonly IMutationDocumentCompiler _compiler;
        private readonly TestRunnerDependencyFilesHandler _testRunnerDependencyFilesHandler;
        private readonly ITestRunnerClient _testRunnerClient;
        private readonly IFileSystem _fileSystem;

        public MutationDocumentExecutor(
            IMutationDocumentCompiler compiler,
            TestRunnerDependencyFilesHandler testRunnerDependencyFilesHandler,
            ITestRunnerClient testRunnerClient,
            IFileSystem fileSystem)
        {
            _compiler = compiler;
            _testRunnerDependencyFilesHandler = testRunnerDependencyFilesHandler;
            _testRunnerClient = testRunnerClient;
            _fileSystem = fileSystem;
        }

        public async Task<MutationDocumentResult> ExecuteMutationAsync(
            MutationConfig config,
            MutationDocument mutationDocument,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var mutationResult = new MutationDocumentResult
            {
                Id = mutationDocument.Id,
                MutationName = mutationDocument.MutationName,
                ProjectName = mutationDocument.ProjectName,
                FileName = mutationDocument.FileName,
                Location = mutationDocument.MutationDetails.Location,
                Orginal = mutationDocument.MutationDetails.Original.ToFullString(),
                FullOrginal = mutationDocument.MutationDetails.FullOriginal.ToFullString(),
                Mutation = mutationDocument.MutationDetails.Mutation.ToFullString(),
                FullMutation = mutationDocument.MutationDetails.FullMutation.ToFullString(),
                Category = mutationDocument.MutationDetails.Category
            };

            mutationResult.GenerateHash();

            Log.Info($"Running mutation: \"{mutationDocument.MutationName}\"");

            var results = new List<TestSuiteResult>();

            // Create the temporary "head" mutation directory to run all tests
            var mutationDirectoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestRun", mutationDocument.Id.ToString());

            // Where we should save our compiled mutation
            var mutationDllPath = Path.Combine(mutationDirectoryPath, $"{config.MutationProjects.FirstOrDefault(m => m.Project.Name == mutationDocument.ProjectName).Project.OutputFileName}");

            _fileSystem.Directory.DeleteDirectoryAndCheckForException(mutationDirectoryPath);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                _fileSystem.Directory.CreateDirectory(mutationDirectoryPath);

                mutationResult.CompilationResult = await _compiler.CompileAsync(mutationDllPath, mutationDocument);
                if (!mutationResult.CompilationResult.IsSuccess)
                {
                    return mutationResult;
                }

                foreach (var testProject in config.TestProjects)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (ShouldIgnoreTestProject(mutationDocument, config.MutationProjects, testProject))
                    {
                        continue;
                    }

                    var baseline = config.BaselineInfos.FirstOrDefault(b => b.TestProjectName.Equals(testProject.Project.Name, StringComparison.OrdinalIgnoreCase));
                    var result = await RunTestAsync(
                        testProject,
                        mutationDirectoryPath,
                        mutationDllPath,
                        config.DotNetPath,
                        baseline?.GetTestProjectTimeout() ?? TimeSpan.FromMinutes(config.MaxTestTimeMin),
                        cancellationToken);

                    results.Add(result);

                    if (results.Any(r => !r.IsSuccess))
                    {
                        break;
                    }
                }

                var final = CombineResult(mutationDocument.FileName, results);

                if (final.TestResults.Count == 0)
                {
                    throw new MutationDocumentException("Unknown error when running, we should not have 0 tests. Also make sure that you don't have bad project mapping.");
                }

                Log.Info($"\"{mutationDocument.MutationName}\" done. Ran {final.TestResults.Count} tests and {final.TestResults.Count(t => !t.IsSuccess)} failed.");

                mutationResult.FailedTests = final.TestResults.Where(t => !t.IsSuccess).ToList();
                mutationResult.TestsRunCount = final.TestResults.Count;
            }
            catch (OperationCanceledException)
            {
                Log.Info("Cancellation requested (single mutation)");
                mutationResult.UnexpectedError = "Mutation cancelled";
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                _fileSystem.Directory.DeleteDirectoryAndCheckForException(mutationDirectoryPath);
            }

            return mutationResult;
        }

        private static bool ShouldIgnoreTestProject(MutationDocument mutationDocument, IList<MutationProject> mutationProjects, TestProject testProject)
        {
            var mutationProject = mutationProjects.FirstOrDefault(m => m.Project.Name == mutationDocument.ProjectName);

            if (mutationProject?.MappedTestProjects != null && mutationProject.MappedTestProjects.Any())
            {
                if (mutationProject.MappedTestProjects.All(m => m != testProject.Project.Name))
                {
                    Log.Info($"Skipping tests in the the test project \"{testProject.Project.Name}\" as " +
                               $"it didn't match the mapping list");
                    return true;
                }
            }

            return false;
        }

        private async Task<TestSuiteResult> RunTestAsync(
            TestProject testProject,
            string mutationDirectoryPath,
            string mutationDllPath,
            string dotNetPath,
            TimeSpan testTimeout,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Log.Info($"Starting to run tests in {testProject.Project.OutputFileName}");
            var testDllPath = _testRunnerDependencyFilesHandler.CreateTestDirectoryAndCopyDependencies(mutationDirectoryPath, testProject, mutationDllPath);

            return await _testRunnerClient.RunTestsAsync(testProject.TestRunner, testDllPath, dotNetPath, testTimeout, cancellationToken);
        }

        private TestSuiteResult CombineResult(string name, IList<TestSuiteResult> testResult)
        {
            var tests = new List<TestResult>();
            var executionTime = TimeSpan.Zero;

            foreach (var testSuiteResult in testResult)
            {
                tests.AddRange(testSuiteResult.TestResults);
                executionTime = executionTime.Add(testSuiteResult.ExecutionTime);
            }

            return new TestSuiteResult
            {
                Name = name,
                TestResults = tests,
                ExecutionTime = executionTime,
                IsSuccess = tests.All(t => t.IsSuccess)
            };
        }
    }
}