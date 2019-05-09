using Cama.Core;
using Cama.Core.Config;
using MediatR;

namespace Cama.Application.Commands.Mutation.ExecuteMutation
{
    public class ExecuteMutationCommand : IRequest<MutationDocumentResult>
    {
        public ExecuteMutationCommand(CamaConfig config, MutationDocument mutation)
        {
            Config = config;
            Mutation = mutation;
        }

        public CamaConfig Config { get; }

        public MutationDocument Mutation { get; }
    }
}
