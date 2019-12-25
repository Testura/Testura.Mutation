using MediatR;
using Testura.Mutation.Application.Models;

namespace Testura.Mutation.Application.Commands.Project.CreateProject
{
    public class CreateProjectCommand : IRequest<bool>
    {
        public CreateProjectCommand(string savePath, TesturaMutationFileConfig config)
        {
            SavePath = savePath;
            Config = config;
        }

        public string SavePath { get; set; }

        public TesturaMutationFileConfig Config { get; set; }
    }
}
