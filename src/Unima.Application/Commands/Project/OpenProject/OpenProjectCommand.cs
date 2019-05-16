using MediatR;
using Unima.Core.Config;

namespace Unima.Application.Commands.Project.OpenProject
{
    public class OpenProjectCommand : IRequest<UnimaConfig>
    {
        public OpenProjectCommand(string path, bool createBaseline)
        {
            Path = path;
            CreateBaseline = createBaseline;
        }

        public string Path { get; set; }

        public bool CreateBaseline { get; set; }
    }
}
