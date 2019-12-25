using System.Threading.Tasks;
using Testura.Mutation.Application.Models;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Solution;

namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers
{
    public class OpenProjectBuildHandler : OpenProjectHandler
    {
        private readonly ISolutionBuilder _solutionBuilder;

        public OpenProjectBuildHandler(ISolutionBuilder solutionBuilder)
        {
            _solutionBuilder = solutionBuilder;
        }

        public override Task HandleAsync(TesturaMutationFileConfig fileConfig, TesturaMutationConfig applicationConfig)
        {
            _solutionBuilder.BuildSolution(fileConfig.SolutionPath);

            return base.HandleAsync(fileConfig, applicationConfig);
        }
    }
}
