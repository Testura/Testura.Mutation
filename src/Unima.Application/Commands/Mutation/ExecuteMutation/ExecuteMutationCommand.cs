using MediatR;
using Unima.Core;
using Unima.Core.Config;

namespace Unima.Application.Commands.Mutation.ExecuteMutation
{
    public class ExecuteMutationCommand : IRequest<MutationDocumentResult>
    {
        public ExecuteMutationCommand(UnimaConfig config, MutationDocument mutation)
        {
            Config = config;
            Mutation = mutation;
        }

        public UnimaConfig Config { get; }

        public MutationDocument Mutation { get; }
    }
}
