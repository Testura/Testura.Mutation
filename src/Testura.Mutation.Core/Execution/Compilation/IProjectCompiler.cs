using System.Threading.Tasks;
using Microsoft.CodeAnalysis;

namespace Testura.Mutation.Core.Execution.Compilation
{
    public interface IProjectCompiler
    {
        Task<CompilationResult> CompileAsync(string directoryPath, Project project);
    }
}