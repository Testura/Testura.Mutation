using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Cama.Core;
using Cama.Core.Creator;

namespace Cama.Service.Commands.Mutation.CreateMutations
{
    public class CreateMutationsCommandHandler : ValidateResponseRequestHandler<CreateMutationsCommand, IList<MutationDocument>>
    {
        private readonly MutationDocumentCreator _mutationsCreator;

        public CreateMutationsCommandHandler(MutationDocumentCreator mutationsCreator)
        {
            _mutationsCreator = mutationsCreator;
        }

        public override async Task<IList<MutationDocument>> OnHandle(CreateMutationsCommand command, CancellationToken cancellationToken)
        {
            return await _mutationsCreator.CreateMutationsAsync(command.Config, command.MutationOperators);
        }
    }
}
