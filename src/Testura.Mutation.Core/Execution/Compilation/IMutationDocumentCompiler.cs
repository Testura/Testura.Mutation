using System.Threading.Tasks;

namespace Testura.Mutation.Core.Execution.Compilation
{
    public interface IMutationDocumentCompiler
    {
        Task<CompilationResult> CompileAsync(string path, MutationDocument document);
    }
}