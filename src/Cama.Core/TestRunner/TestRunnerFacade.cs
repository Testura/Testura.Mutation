using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Cama.Core.Config;
using Cama.Core.Mutation.Models;
using Cama.Core.Services;
using Cama.Core.Solution;
using Cama.Core.TestRunner.Result;
using Cama.Core.TestRunner.Runners;
using File = System.IO.File;

namespace Cama.Core.TestRunner
{
    public class TestRunnerFacade
    {
        private readonly MutatedDocumentCompiler _compiler;
        private readonly TestRunnerDependencyFilesHandler _testRunnerDependencyFilesHandler;
        private readonly NUnitTestRunner _testRunner;

        public TestRunnerFacade(MutatedDocumentCompiler compiler, TestRunnerDependencyFilesHandler testRunnerDependencyFilesHandler, NUnitTestRunner testRunner)
        {
            _compiler = compiler;
            _testRunnerDependencyFilesHandler = testRunnerDependencyFilesHandler;
            _testRunner = testRunner;
        }

        public async Task<MutationDocumentResult> RunTestAsync(CamaConfig config, MutationDocument document)
        {
            var mustationResult = new MutationDocumentResult
            {
                MutationName = document.MutationName,
                ProjectName = document.ProjectName,
                FileName = document.FileName,
                Location = document.MutationDetails.Location,
                Orginal = document.MutationDetails.Orginal.ToFullString(),
                FullOrginal = document.MutationDetails.FullOrginal.ToFullString(),
                Mutation = document.MutationDetails.Mutation.ToFullString(),
                FullMutation = document.MutationDetails.FullMutation.ToFullString()
            };

            LogTo.Info($"Running mutation: \"{document.MutationName}\"");

            var results = new List<TestSuiteResult>();
            var basePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestRun", document.Id.ToString());
            var mainFilePath = Path.Combine(basePath, config.MutationProjects.FirstOrDefault(m => m.Name == document.ProjectName).Name);
            Directory.CreateDirectory(basePath);

            mustationResult.CompilerResult = await _compiler.CompileAsync(mainFilePath, document);
            if (!mustationResult.CompilerResult.IsSuccess)
            {
                return mustationResult;
            }

            foreach (var testProject in config.TestProjects)
            {
                await RunTestAsync(results, basePath, mainFilePath, testProject, config.MaxTestTimeMin);

                if (results.Any(r => !r.IsSuccess))
                {
                    break;
                }
            }

            try
            {
                Directory.Delete(basePath, true);
            }
            catch (Exception ex)
            {
                LogTo.Error($"Failed to delete test directory: {ex.Message}");
            }

            var final = CombineResult(document.FileName, results);
            LogTo.Info($"\"{document.MutationName}\" done. Ran {final.TestResults.Count} tests and {final.TestResults.Count(t => !t.IsSuccess)} failed.");

            mustationResult.FailedTests = final.TestResults.Where(t => !t.IsSuccess).ToList();
            mustationResult.TestsRunCount = final.TestResults.Count;

            return mustationResult;
        }

        private async Task RunTestAsync(List<TestSuiteResult> results, string basePath, string mainFilePath, SolutionProjectInfo testProject, int maxTestTimeMin)
        {
            LogTo.Info($"Starting to run tests in {testProject.OutputFileName}");
            var baseTestPath = Path.Combine(basePath, Guid.NewGuid().ToString());
            var mainTestFilePath = Path.Combine(baseTestPath, testProject.OutputFileName);
            Directory.CreateDirectory(baseTestPath);

            _testRunnerDependencyFilesHandler.CopyDependencies(testProject.OutputFileName, baseTestPath);
            File.Copy(mainFilePath, Path.Combine(baseTestPath, Path.GetFileName(mainFilePath)), true);
            results.Add(await _testRunner.RunTestsAsync(mainTestFilePath, /* document.Document.Tests */ new List<string>(), TimeSpan.FromMinutes(maxTestTimeMin)));
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