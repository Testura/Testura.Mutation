using System.Threading.Tasks;

namespace Cama.Core.Execution.Compilation
{
    public interface IMutationDocumentCompiler
    {
        Task<CompilationResult> CompileAsync(string path, MutationDocument document);
    }
}