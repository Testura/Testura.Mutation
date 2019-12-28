using MediatR;
using Testura.Mutation.Application.Models;

namespace Testura.Mutation.Application.Commands.Project.CreateProject
{
    public class CreateProjectCommand : IRequest<bool>
    {
        public CreateProjectCommand(string savePath, MutationFileConfig config)
        {
            SavePath = savePath;
            Config = config;
        }

        public string SavePath { get; set; }

        public MutationFileConfig Config { get; set; }
    }
}
