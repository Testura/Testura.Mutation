using MediatR;
using Unima.Application.Models;
using Unima.Core.Config;

namespace Unima.Application.Commands.Project.OpenProject
{
    public class OpenProjectCommand : IRequest<UnimaConfig>
    {
        public OpenProjectCommand(string path)
        {
            Path = path;
        }

        public OpenProjectCommand(UnimaFileConfig config)
        {
            Config = config;
        }

        public string Path { get; set; }

        public UnimaFileConfig Config { get; set; }
    }
}
