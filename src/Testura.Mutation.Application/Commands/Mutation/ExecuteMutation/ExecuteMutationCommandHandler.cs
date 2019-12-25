using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Execution;

namespace Testura.Mutation.Application.Commands.Mutation.ExecuteMutation
{
    public class ExecuteMutationCommandHandler : IRequestHandler<ExecuteMutationCommand, MutationDocumentResult>
    {
        private readonly MutationDocumentExecutor _mutationDocumentExecutor;

        public ExecuteMutationCommandHandler(MutationDocumentExecutor mutationDocumentExecutor)
        {
            _mutationDocumentExecutor = mutationDocumentExecutor;
        }

        public async Task<MutationDocumentResult> Handle(ExecuteMutationCommand command, CancellationToken cancellationToken)
        {
            return await _mutationDocumentExecutor.ExecuteMutationAsync(command.Config, command.Mutation, cancellationToken);
        }
    }
}
