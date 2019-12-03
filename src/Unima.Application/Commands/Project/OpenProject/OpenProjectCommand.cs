using MediatR;
using Unima.Core.Config;

namespace Unima.Application.Commands.Project.OpenProject
{
    public class OpenProjectCommand : IRequest<UnimaConfig>
    {
        public OpenProjectCommand(string path)
        {
            Path = path;
        }

        public string Path { get; set; }
    }
}
