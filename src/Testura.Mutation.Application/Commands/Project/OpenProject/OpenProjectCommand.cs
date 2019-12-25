using MediatR;
using Testura.Mutation.Application.Models;
using Testura.Mutation.Core.Config;

namespace Testura.Mutation.Application.Commands.Project.OpenProject
{
    public class OpenProjectCommand : IRequest<TesturaMutationConfig>
    {
        public OpenProjectCommand(string path)
        {
            Path = path;
        }

        public OpenProjectCommand(TesturaMutationFileConfig config)
        {
            Config = config;
        }

        public string Path { get; set; }

        public TesturaMutationFileConfig Config { get; set; }
    }
}
