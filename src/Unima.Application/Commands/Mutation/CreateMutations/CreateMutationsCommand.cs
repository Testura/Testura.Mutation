using System.Collections.Generic;
using MediatR;
using Unima.Core;
using Unima.Core.Config;
using Unima.Core.Creator.Mutators;

namespace Unima.Application.Commands.Mutation.CreateMutations
{
    public class CreateMutationsCommand : IRequest<IList<MutationDocument>>
    {
        public CreateMutationsCommand(UnimaConfig config, IList<IMutator> mutationOperators)
        {
            Config = config;
            MutationOperators = mutationOperators;
        }

        public UnimaConfig Config { get; set; }

        public IList<IMutator> MutationOperators { get; set; }
    }
}
