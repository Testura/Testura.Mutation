using Cama.Core;
using Cama.Core.Execution.Compilation;
using MediatR;

namespace Cama.Application.Commands.Mutation.CompileMutation
{
    public class CompileMutationCommand : IRequest<CompilationResult>
    {
        public string SavePath { get; set; }

        public MutationDocument Mutation { get; set; }
    }
}
