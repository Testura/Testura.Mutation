using Cama.Application.Models;
using MediatR;

namespace Cama.Application.Commands.Project.CreateProject
{
    public class CreateProjectCommand : IRequest<bool>
    {
        public CreateProjectCommand(string savePath, CamaFileConfig config)
        {
            SavePath = savePath;
            Config = config;
        }

        public string SavePath { get; set; }

        public CamaFileConfig Config { get; set; }
    }
}
