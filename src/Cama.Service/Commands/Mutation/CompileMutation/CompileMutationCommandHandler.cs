using System.Threading;
using System.Threading.Tasks;
using Cama.Core.Execution.Compilation;

namespace Cama.Service.Commands.Mutation.CompileMutation
{
    public class CompileMutationCommandHandler : ValidateResponseRequestHandler<CompileMutationCommand, CompilationResult>
    {
        private readonly MutationDocumentCompiler _compiler;

        public CompileMutationCommandHandler(MutationDocumentCompiler compiler)
        {
            _compiler = compiler;
        }

        public override async Task<CompilationResult> OnHandle(CompileMutationCommand command, CancellationToken cancellationToken)
        {
            return await _compiler.CompileAsync(command.SavePath, command.Mutation);
        }
    }
}
