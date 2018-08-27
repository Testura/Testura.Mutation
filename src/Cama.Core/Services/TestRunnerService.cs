using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Cama.Core.Models;
using Cama.Core.Models.Mutation;
using Cama.Core.Models.Project;
using File = System.IO.File;

namespace Cama.Core.Services
{
    public class TestRunnerService
    {
        private readonly MutatedDocumentCompiler _compiler;
        private readonly DependencyFilesHandler _dependencyFilesHandler;
        private readonly TestRunner.TestRunner _testRunner;

        public TestRunnerService(MutatedDocumentCompiler compiler, DependencyFilesHandler dependencyFilesHandler, TestRunner.TestRunner testRunner)
        {
            _compiler = compiler;
            _dependencyFilesHandler = dependencyFilesHandler;
            _testRunner = testRunner;
        }

        public async Task<MutationDocumentResult> RunTestAsync(CamaRunConfig config, MutatedDocument document)
        {
            LogTo.Info($"Running mutation: \"{document.MutationName}\"");

            var results = new List<TestSuiteResult>();
            var basePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestRun", document.Id.ToString());
            var mainFilePath = Path.Combine(basePath, config.MutationProjects.FirstOrDefault(m => m.MutationProjectName == document.ProjectName).MutationProjectOutputFileName);
            Directory.CreateDirectory(basePath);

            var compilerResult = await _compiler.CompileAsync(mainFilePath, document);
            if (!compilerResult.IsSuccess)
            {
                return new MutationDocumentResult { Survived = false, CompilerResult = compilerResult, Document = document };
            }

            foreach (var testProject in config.TestProjects)
            {
                var baseTestPath = Path.Combine(basePath, Guid.NewGuid().ToString());
                var mainTestFilePath = Path.Combine(baseTestPath, testProject.TestProjectOutputFileName);
                Directory.CreateDirectory(baseTestPath);

                _dependencyFilesHandler.CopyDependencies(testProject.TestProjectOutputPath, baseTestPath);
                File.Copy(mainFilePath, Path.Combine(baseTestPath, Path.GetFileName(mainFilePath)), true);
                results.Add(_testRunner.RunTests(mainTestFilePath, /* document.Document.Tests */ new List<string>()));
            }

            try
            {
                Directory.Delete(basePath, true);
            }
            catch (Exception)
            {
            }

            var final = CombineResult(document.FileName, results);
            LogTo.Info($"\"{document.MutationName}\" done.");
            return new MutationDocumentResult { Survived = final.IsSuccess, CompilerResult = compilerResult, TestResult = final, Document = document };
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