using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Unima.Core.Config;
using Unima.Core.Exceptions;
using Unima.Core.Execution.Compilation;
using Unima.Core.Execution.Result;
using Unima.Core.Execution.Runners;

namespace Unima.Core.Execution
{
    public class MutationDocumentExecutor
    {
        private readonly IMutationDocumentCompiler _compiler;
        private readonly TestRunnerDependencyFilesHandler _testRunnerDependencyFilesHandler;
        private readonly ITestRunnerClient _testRunnerClient;

        public MutationDocumentExecutor(IMutationDocumentCompiler compiler, TestRunnerDependencyFilesHandler testRunnerDependencyFilesHandler, ITestRunnerClient testRunnerClient)
        {
            _compiler = compiler;
            _testRunnerDependencyFilesHandler = testRunnerDependencyFilesHandler;
            _testRunnerClient = testRunnerClient;
        }

        public async Task<MutationDocumentResult> ExecuteMutationAsync(
            UnimaConfig config,
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
                Orginal = mutationDocument.MutationDetails.Orginal.ToFullString(),
                FullOrginal = mutationDocument.MutationDetails.FullOrginal.ToFullString(),
                Mutation = mutationDocument.MutationDetails.Mutation.ToFullString(),
                FullMutation = mutationDocument.MutationDetails.FullMutation.ToFullString()
            };

            mutationResult.GenerateHash();

            LogTo.Info($"Running mutation: \"{mutationDocument.MutationName}\"");

            var results = new List<TestSuiteResult>();

            // Create the temporary "head" mutation directory to run all tests
            var mutationDirectoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestRun", mutationDocument.Id.ToString());

            // Where we should save our compiled mutation
            var mutationDllPath = Path.Combine(mutationDirectoryPath, $"{config.MutationProjects.FirstOrDefault(m => m.Name == mutationDocument.ProjectName).OutputFileName}");

            Directory.CreateDirectory(mutationDirectoryPath);

            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                mutationResult.CompilationResult = await _compiler.CompileAsync(mutationDllPath, mutationDocument);
                if (!mutationResult.CompilationResult.IsSuccess)
                {
                    return mutationResult;
                }

                foreach (var testProject in config.TestProjects)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var baseline = config.BaselineInfos.FirstOrDefault(b => b.TestProjectName.Equals(testProject.Project.Name, StringComparison.OrdinalIgnoreCase));
                    var result = await RunTestAsync(testProject, mutationDirectoryPath, mutationDllPath, config.DotNetPath, baseline?.GetTestProjectTimeout() ?? TimeSpan.FromMinutes(config.MaxTestTimeMin));
                    results.Add(result);

                    if (results.Any(r => !r.IsSuccess))
                    {
                        break;
                    }
                }

                var final = CombineResult(mutationDocument.FileName, results);

                if (final.TestResults.Count == 0)
                {
                    throw new MutationDocumentException("Unknown error when running, we should not have 0 tests.");
                }

                LogTo.Info($"\"{mutationDocument.MutationName}\" done. Ran {final.TestResults.Count} tests and {final.TestResults.Count(t => !t.IsSuccess)} failed.");

                mutationResult.FailedTests = final.TestResults.Where(t => !t.IsSuccess).ToList();
                mutationResult.TestsRunCount = final.TestResults.Count;
            }
            catch (OperationCanceledException)
            {
                LogTo.Info("Cancellation requested (single mutation)");
                mutationResult.UnexpectedError = "Mutation cancelled";
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                DeleteMutationDirectory(mutationDirectoryPath);
            }

            return mutationResult;
        }

        private async Task<TestSuiteResult> RunTestAsync(
            TestProject testProject,
            string mutationDirectoryPath,
            string mutationDllPath,
            string dotNetPath,
            TimeSpan testTimout)
        {
            LogTo.Info($"Starting to run tests in {testProject.Project.OutputFileName}");
            var mutationTestDirectoryPath = Path.Combine(mutationDirectoryPath, Guid.NewGuid().ToString());
            var testDllPath = Path.Combine(mutationTestDirectoryPath, testProject.Project.OutputFileName);
            Directory.CreateDirectory(mutationTestDirectoryPath);

            // Copy all files from the test directory to our own mutation test directory
            _testRunnerDependencyFilesHandler.CopyDependencies(testProject.Project.OutputDirectoryPath, mutationTestDirectoryPath);

            // Copy the mutation to our mutation test directory (and override the orginal file)
            File.Copy(mutationDllPath, Path.Combine(mutationTestDirectoryPath, Path.GetFileName(mutationDllPath)), true);

            return await _testRunnerClient.RunTestsAsync(testProject.TestRunner, testDllPath, dotNetPath, testTimout);
        }

        private void DeleteMutationDirectory(string mutationDirectoryPath)
        {
            try
            {
                Directory.Delete(mutationDirectoryPath, true);
            }
            catch (Exception ex)
            {
                LogTo.Error($"Failed to delete test directory: {ex.Message}");
            }
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