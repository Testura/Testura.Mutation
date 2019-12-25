using MediatR;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Config;

namespace Testura.Mutation.Application.Commands.Mutation.ExecuteMutation
{
    public class ExecuteMutationCommand : IRequest<MutationDocumentResult>
    {
        public ExecuteMutationCommand(TesturaMutationConfig config, MutationDocument mutation)
        {
            Config = config;
            Mutation = mutation;
        }

        public TesturaMutationConfig Config { get; }

        public MutationDocument Mutation { get; }
    }
}
