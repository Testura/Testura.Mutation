using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Testura.Mutation.Core.Execution.Compilation;

namespace Testura.Mutation.Tests.Utils.Stubs
{
    public class ProjectCompilerStub : IProjectCompiler
    {
        private readonly CompilationResult _compilationResult;

        public ProjectCompilerStub(CompilationResult compilationResult)
        {
            _compilationResult = compilationResult;
        }

        public Task<CompilationResult> CompileAsync(string directoryPath, Project project)
        {
            return Task.FromResult(_compilationResult);
        }
    }
}
