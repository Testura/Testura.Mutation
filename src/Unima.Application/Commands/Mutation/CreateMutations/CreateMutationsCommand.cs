using System.Collections.Generic;
using MediatR;
using Unima.Core;
using Unima.Core.Config;

namespace Unima.Application.Commands.Mutation.CreateMutations
{
    public class CreateMutationsCommand : IRequest<IList<MutationDocument>>
    {
        public CreateMutationsCommand(UnimaConfig config)
        {
            Config = config;
        }

        public UnimaConfig Config { get; set; }
    }
}
