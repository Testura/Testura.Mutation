using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Cama.Core.Models.Mutation;
using Cama.Core.Services;
using Cama.Core.TestRunner;
using Cama.Module.Mutation.Models;

namespace Cama.Module.Mutation.Services
{
    public class TestRunnerService
    {
        private readonly MutatedDocumentCompiler _compiler;
        private readonly DependencyFilesHandler _dependencyFilesHandler;
        private readonly TestRunner _testRunner;

        public TestRunnerService(MutatedDocumentCompiler compiler, DependencyFilesHandler dependencyFilesHandler,
            TestRunner testRunner)
        {
            _compiler = compiler;
            _dependencyFilesHandler = dependencyFilesHandler;
            _testRunner = testRunner;
        }

        public async Task<MutationDocumentResult> RunTestAsync(TestRunDocument document, string sourcePath)
        {
            var basePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "TestRun",
                document.Document.Id.ToString());
            var mainFilePath = Path.Combine(basePath, "Testura.Code.dll");
            var mainTestFilePath = Path.Combine(basePath, "Testura.Code.Tests.dll");

            Directory.CreateDirectory(basePath);

                        document.Status = TestRunDocument.TestRunStatusEnum.CopyFiles;
            _dependencyFilesHandler.CopyDependencies(sourcePath, basePath);

            document.Status = TestRunDocument.TestRunStatusEnum.Compiling;
            var compilerResult = await _compiler.CompileAsync(mainFilePath, document.Document);
            if (!compilerResult.IsSuccess)
            {
                return new MutationDocumentResult { Survived = false, CompilerResult = compilerResult, Document = document.Document };
            }

            document.Status = TestRunDocument.TestRunStatusEnum.Running;
            var results =
                _testRunner.RunTests(mainTestFilePath, /* document.Document.Tests */ new List<string>());

            Directory.Delete(basePath, true);

            return new MutationDocumentResult { Survived = results.IsSuccess, CompilerResult = compilerResult, TestResult = results, Document = document.Document };
        }
    }
}