using System.Threading;
using System.Threading.Tasks;
using Testura.Mutation.Application.Models;
using Testura.Mutation.Core.Config;

namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers
{
    public abstract class OpenProjectHandler
    {
        public OpenProjectHandler Next { get; set; }

        public virtual Task HandleAsync(MutationFileConfig fileConfig, MutationConfig applicationConfig, CancellationToken cancellationToken = default(CancellationToken))
        {
            return Next?.HandleAsync(fileConfig, applicationConfig, cancellationToken) ?? Task.CompletedTask;
        }

        public OpenProjectHandler SetNext(OpenProjectHandler handler)
        {
            Next = handler;
            return Next;
        }
    }
}
