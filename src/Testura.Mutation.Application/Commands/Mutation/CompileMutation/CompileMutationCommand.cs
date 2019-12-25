using MediatR;
using Testura.Mutation.Core;
using Testura.Mutation.Core.Execution.Compilation;

namespace Testura.Mutation.Application.Commands.Mutation.CompileMutation
{
    public class CompileMutationCommand : IRequest<CompilationResult>
    {
        public string SavePath { get; set; }

        public MutationDocument Mutation { get; set; }
    }
}
