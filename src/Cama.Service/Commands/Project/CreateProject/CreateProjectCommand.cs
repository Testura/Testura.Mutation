using Cama.Service.Models;
using MediatR;

namespace Cama.Service.Commands.Project.CreateProject
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
