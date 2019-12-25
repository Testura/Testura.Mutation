using System.Collections.Generic;
using MediatR;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Config;

namespace Testura.Mutation.Application.Commands.Mutation.CreateMutations
{
    public class CreateMutationsCommand : IRequest<IList<MutationDocument>>
    {
        public CreateMutationsCommand(TesturaMutationConfig config)
        {
            Config = config;
        }

        public TesturaMutationConfig Config { get; set; }
    }
}
