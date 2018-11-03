using System.Threading;
using System.Threading.Tasks;
using Cama.Core.Execution.Compilation;
using MediatR;

namespace Cama.Service.Commands.Mutation.CompileMutation
{
    public class CompileMutationCommandHandler : IRequestHandler<CompileMutationCommand, CompilationResult>
    {
        private readonly MutationDocumentCompiler _compiler;

        public CompileMutationCommandHandler(MutationDocumentCompiler compiler)
        {
            _compiler = compiler;
        }

        public async Task<CompilationResult> Handle(CompileMutationCommand command, CancellationToken cancellationToken)
        {
            return await _compiler.CompileAsync(command.SavePath, command.Mutation);
        }
    }
}
