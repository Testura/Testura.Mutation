using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Creator;

namespace Testura.Mutation.Application.Commands.Mutation.CreateMutations
{
    public class CreateMutationsCommandHandler : IRequestHandler<CreateMutationsCommand, IList<MutationDocument>>
    {
        private readonly MutationDocumentCreator _mutationsCreator;

        public CreateMutationsCommandHandler(MutationDocumentCreator mutationsCreator)
        {
            _mutationsCreator = mutationsCreator;
        }

        public Task<IList<MutationDocument>> Handle(CreateMutationsCommand command, CancellationToken cancellationToken)
        {
            return Task.FromResult(_mutationsCreator.CreateMutations(command.Config, cancellationToken));
        }
    }
}
