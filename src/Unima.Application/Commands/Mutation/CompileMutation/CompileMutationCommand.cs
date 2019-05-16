using MediatR;
using Unima.Core;
using Unima.Core.Execution.Compilation;

namespace Unima.Application.Commands.Mutation.CompileMutation
{
    public class CompileMutationCommand : IRequest<CompilationResult>
    {
        public string SavePath { get; set; }

        public MutationDocument Mutation { get; set; }
    }
}
