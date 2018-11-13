using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Cama.Core.Execution.Compilation;
using Cama.Core.Execution.Result;
using Cama.Core.Execution.Runners;
using Cama.Core.Solution;

namespace Cama.Core.Execution
{
    public class MutationDocumentExecutor
    {
        private readonly MutationDocumentCompiler _compiler;
        private readonly TestRunnerDependencyFilesHandler _testRunnerDependencyFilesHandler;
        private readonly ITestRunnerFactory _testRunnerFactory;

        public MutationDocumentExecutor(MutationDocumentCompiler compiler, TestRunnerDependencyFilesHandler testRunnerDependencyFilesHandler, ITestRunnerFactory testRunnerFactory)
        {
            _compiler = compiler;
            _testRunnerDependencyFilesHandler = testRunnerDependencyFilesHandler;
            _testRunnerFactory = testRunnerFactory;
        }

        public async Task<MutationDocumentResult> ExecuteMutationAsync(CamaConfig config, MutationDocument mutationDocument)
        {
            var mustationResult = new MutationDocumentResult
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

            LogTo.Info($"Running mutation: \"{mutationDocument.MutationName}\"");

            var results = new List<TestSuiteResult>();

            // Create the temporary "head" mutation directory to run all tests
            var mutationDirectoryPath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestRun", mutationDocument.Id.ToString());

            // Where we should save our compiled mutation
            var mutationDllPath = Path.Combine(mutationDirectoryPath, $"{config.MutationProjects.FirstOrDefault(m => m.Name == mutationDocument.ProjectName).OutputFileName}");

            Directory.CreateDirectory(mutationDirectoryPath);

            mustationResult.CompilationResult = await _compiler.CompileAsync(mutationDllPath, mutationDocument);
            if (!mustationResult.CompilationResult.IsSuccess)
            {
                return mustationResult;
            }

            foreach (var testProject in config.TestProjects)
            {
                await RunTestAsync(config.TestRunner, results, mutationDirectoryPath, mutationDllPath, testProject, config.MaxTestTimeMin);

                if (results.Any(r => !r.IsSuccess))
                {
                    break;
                }
            }

            try
            {
                Directory.Delete(mutationDirectoryPath, true);
            }
            catch (Exception ex)
            {
                LogTo.Error($"Failed to delete test directory: {ex.Message}");
            }

            var final = CombineResult(mutationDocument.FileName, results);
            LogTo.Info($"\"{mutationDocument.MutationName}\" done. Ran {final.TestResults.Count} tests and {final.TestResults.Count(t => !t.IsSuccess)} failed.");

            mustationResult.FailedTests = final.TestResults.Where(t => !t.IsSuccess).ToList();
            mustationResult.TestsRunCount = final.TestResults.Count;

            return mustationResult;
        }

        private async Task RunTestAsync(
            string testRunnerName,
            List<TestSuiteResult> results,
            string mutationDirectoryPath,
            string mutationDllPath,
            SolutionProjectInfo testProject,
            int maxTestTimeMin)
        {
            LogTo.Info($"Starting to run tests in {testProject.OutputFileName}");
            var mutationTestDirectoryPath = Path.Combine(mutationDirectoryPath, Guid.NewGuid().ToString());
            var testDllPath = Path.Combine(mutationTestDirectoryPath, testProject.OutputFileName);
            Directory.CreateDirectory(mutationTestDirectoryPath);

            // Copy all files from the test directory to our own mutation test directory
            _testRunnerDependencyFilesHandler.CopyDependencies(testProject.OutputDirectoryPath, mutationTestDirectoryPath);

            // Copy the mutation to our mutation test directory (and override the orginal file)
            File.Copy(mutationDllPath, Path.Combine(mutationTestDirectoryPath, Path.GetFileName(mutationDllPath)), true);

            var testRunner = _testRunnerFactory.CreateTestRunner(testRunnerName);
            results.Add(await testRunner.RunTestsAsync(testDllPath, TimeSpan.FromMinutes(maxTestTimeMin)));
        }

        private TestSuiteResult CombineResult(string name, IList<TestSuiteResult> testResult)
        {
            var tests = new List<TestResult>();
            foreach (var testSuiteResult in testResult)
            {
                tests.AddRange(testSuiteResult.TestResults);
            }

            return new TestSuiteResult(name, tests, null);
        }
    }
}