using Cama.Core;
using MediatR;

namespace Cama.Service.Commands.Project.OpenProject
{
    public class OpenProjectCommand : IRequest<CamaConfig>
    {
        public OpenProjectCommand(string path)
        {
            Path = path;
        }

        public string Path { get; set; }
    }
}
