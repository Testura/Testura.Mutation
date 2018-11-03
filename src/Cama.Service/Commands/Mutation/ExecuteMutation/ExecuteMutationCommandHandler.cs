using System.Threading;
using System.Threading.Tasks;
using Cama.Core;
using Cama.Core.Execution;
using MediatR;

namespace Cama.Service.Commands.Mutation.ExecuteMutation
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
            return await _mutationDocumentExecutor.ExecuteMutationAsync(command.Config, command.Mutation);
        }
    }
}
