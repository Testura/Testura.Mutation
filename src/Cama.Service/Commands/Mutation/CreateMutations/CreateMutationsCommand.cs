using System.Collections.Generic;
using Cama.Core;
using Cama.Core.Creator.Mutators;
using MediatR;

namespace Cama.Service.Commands.Mutation.CreateMutations
{
    public class CreateMutationsCommand : IRequest<IList<MutationDocument>>
    {
        public CreateMutationsCommand(CamaConfig config, IList<IMutator> mutationOperators)
        {
            Config = config;
            MutationOperators = mutationOperators;
        }

        public CamaConfig Config { get; set; }

        public IList<IMutator> MutationOperators { get; set; }
    }
}
