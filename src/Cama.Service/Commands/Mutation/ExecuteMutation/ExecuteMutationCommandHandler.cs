using System.Threading;
using System.Threading.Tasks;
using Cama.Core;
using Cama.Core.Execution;

namespace Cama.Service.Commands.Mutation.ExecuteMutation
{
    public class ExecuteMutationCommandHandler : ValidateResponseRequestHandler<ExecuteMutationCommand, MutationDocumentResult>
    {
        private readonly MutationDocumentExecutor _mutationDocumentExecutor;

        public ExecuteMutationCommandHandler(MutationDocumentExecutor mutationDocumentExecutor)
        {
            _mutationDocumentExecutor = mutationDocumentExecutor;
        }

        public override async Task<MutationDocumentResult> OnHandle(ExecuteMutationCommand command, CancellationToken cancellationToken)
        {
            return await _mutationDocumentExecutor.ExecuteMutationAsync(command.Config, command.Mutation);
        }
    }
}
