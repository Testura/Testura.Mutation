using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using CompilationError = Cama.Core.Models.CompilationError;
using CompilationResult = Cama.Core.Models.CompilationResult;
using MutatedDocument = Cama.Core.Models.Mutation.MutatedDocument;

namespace Cama.Core.Services
{
    public class MutatedDocumentCompiler
    {
        public async Task<CompilationResult> CompileAsync(string path, MutatedDocument document)
        {
            var compilation = await document.CreateMutatedDocument().Project.GetCompilationAsync();
            var result = compilation.Emit(path);
            return new CompilationResult
            {
                IsSuccess = result.Success,
                Errors = result.Diagnostics
                    .Where(d => d.Severity == DiagnosticSeverity.Error)
                    .Select(d => new CompilationError { Message = d.GetMessage(), Location = d.Location.ToString() })
                    .ToList()
            };
        }
    }
}
