using System.Threading.Tasks;
using Unima.Application.Models;
using Unima.Core.Config;
using Unima.Core.Solution;

namespace Unima.Application.Commands.Project.OpenProject.Handlers
{
    public class OpenProjectBuildHandler : OpenProjectHandler
    {
        private readonly ISolutionBuilder _solutionBuilder;

        public OpenProjectBuildHandler(ISolutionBuilder solutionBuilder)
        {
            _solutionBuilder = solutionBuilder;
        }

        public override Task HandleAsync(UnimaFileConfig fileConfig, UnimaConfig applicationConfig)
        {
            _solutionBuilder.BuildSolution(fileConfig.SolutionPath);

            return base.HandleAsync(fileConfig, applicationConfig);
        }
    }
}
