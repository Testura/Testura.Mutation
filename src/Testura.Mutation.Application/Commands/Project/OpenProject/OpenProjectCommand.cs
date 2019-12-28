using MediatR;
using Testura.Mutation.Application.Models;
using Testura.Mutation.Core.Config;

namespace Testura.Mutation.Application.Commands.Project.OpenProject
{
    public class OpenProjectCommand : IRequest<MutationConfig>
    {
        public OpenProjectCommand(string path)
        {
            Path = path;
        }

        public OpenProjectCommand(MutationFileConfig config)
        {
            Config = config;
        }

        public string Path { get; set; }

        public MutationFileConfig Config { get; set; }
    }
}
