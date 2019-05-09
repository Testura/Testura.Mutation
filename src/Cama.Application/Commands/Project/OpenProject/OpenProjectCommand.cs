using Cama.Core.Config;
using MediatR;

namespace Cama.Application.Commands.Project.OpenProject
{
    public class OpenProjectCommand : IRequest<CamaConfig>
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
