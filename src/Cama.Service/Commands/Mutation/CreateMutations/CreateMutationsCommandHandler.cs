using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cama.Core;
using Cama.Core.Creator;
using MediatR;

namespace Cama.Service.Commands.Mutation.CreateMutations
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
            return _mutationsCreator.CreateMutationsAsync(command.Config, command.MutationOperators);
        }
    }
}
