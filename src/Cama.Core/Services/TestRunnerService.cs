using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Cama.Core.Models.Mutation;

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

        public async Task<MutationDocumentResult> RunTestAsync(CamaConfig config, MutatedDocument document, string sourcePath)
        {
            var basePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestRun", document.Id.ToString());
            var mainFilePath = Path.Combine(basePath, config.MutationProjectOutputFileName);
            var mainTestFilePath = Path.Combine(basePath, config.TestProjectOutputFileName);

            Directory.CreateDirectory(basePath);
            _dependencyFilesHandler.CopyDependencies(sourcePath, basePath);
            var compilerResult = await _compiler.CompileAsync(mainFilePath, document);
            if (!compilerResult.IsSuccess)
            {
                return new MutationDocumentResult { Survived = false, CompilerResult = compilerResult, Document = document };
            }

            var results = _testRunner.RunTests(mainTestFilePath, /* document.Document.Tests */ new List<string>());

            try
            {
                Directory.Delete(basePath, true);
            }
            catch (Exception)
            {
            }

            return new MutationDocumentResult { Survived = results.IsSuccess, CompilerResult = compilerResult, TestResult = results, Document = document };
        }
    }
}