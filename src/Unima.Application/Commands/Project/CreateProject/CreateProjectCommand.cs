using MediatR;
using Unima.Application.Models;

namespace Unima.Application.Commands.Project.CreateProject
{
    public class CreateProjectCommand : IRequest<bool>
    {
        public CreateProjectCommand(string savePath, UnimaFileConfig config)
        {
            SavePath = savePath;
            Config = config;
        }

        public string SavePath { get; set; }

        public UnimaFileConfig Config { get; set; }
    }
}
