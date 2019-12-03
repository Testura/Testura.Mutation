using System.Threading.Tasks;
using Unima.Application.Models;
using Unima.Core.Config;

namespace Unima.Application.Commands.Project.OpenProject.Handlers
{
    public abstract class OpenProjectHandler
    {
        public OpenProjectHandler Next { get; set; }

        public virtual Task HandleAsync(UnimaFileConfig fileConfig, UnimaConfig applicationConfig)
        {
            return Next?.HandleAsync(fileConfig, applicationConfig) ?? Task.CompletedTask;
        }

        public OpenProjectHandler SetNext(OpenProjectHandler handler)
        {
            Next = handler;
            return Next;
        }
    }
}
