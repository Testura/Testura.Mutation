using System.Threading.Tasks;

namespace Unima.Core.Execution.Compilation
{
    public interface IMutationDocumentCompiler
    {
        Task<CompilationResult> CompileAsync(string path, MutationDocument document);
    }
}