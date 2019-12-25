using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Testura.Mutation.Core.Execution.Compilation;

namespace Testura.Mutation.Application.Commands.Mutation.CompileMutation
{
    public class CompileMutationCommandHandler : IRequestHandler<CompileMutationCommand, CompilationResult>
    {
        private readonly Compiler _compiler;

        public CompileMutationCommandHandler(Compiler compiler)
        {
            _compiler = compiler;
        }

        public async Task<CompilationResult> Handle(CompileMutationCommand command, CancellationToken cancellationToken)
        {
            return await _compiler.CompileAsync(command.SavePath, command.Mutation);
        }
    }
}
