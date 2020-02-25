using System.IO;
using System.IO.Abstractions;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Execution.Compilation;

namespace Testura.Mutation.Tests.Utils.Stubs
{
    public class ProjectCompilerStub : IProjectCompiler, IMutationDocumentCompiler
    {
        private readonly CompilationResult _compilationResult;
        private readonly IFileSystem _fileSystem;

        public ProjectCompilerStub(CompilationResult compilationResult, IFileSystem fileSystem)
        {
            _compilationResult = compilationResult;
            _fileSystem = fileSystem;
        }

        public Task<CompilationResult> CompileAsync(string directoryPath, Project project)
        {
            var path = Path.Combine(directoryPath, Path.GetFileName(project.OutputFilePath));
            _fileSystem.File.AppendAllText(path, "test");

            return Task.FromResult(_compilationResult);
        }

        public Task<CompilationResult> CompileAsync(string path, MutationDocument document)
        {
            _fileSystem.File.AppendAllText(path, "test");
            return Task.FromResult(_compilationResult);
        }
    }
}
