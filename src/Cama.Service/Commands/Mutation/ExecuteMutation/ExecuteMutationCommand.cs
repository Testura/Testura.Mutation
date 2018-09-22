using System;
using Cama.Core;
using MediatR;

namespace Cama.Service.Commands.Mutation.ExecuteMutation
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
