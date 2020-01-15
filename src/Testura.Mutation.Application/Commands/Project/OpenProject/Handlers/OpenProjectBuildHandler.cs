using System.Threading;
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

        public override Task HandleAsync(MutationFileConfig fileConfig, MutationConfig applicationConfig, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (fileConfig.BuildSolution)
            {
                _solutionBuilder.BuildSolution(fileConfig.SolutionPath);
            }

            return base.HandleAsync(fileConfig, applicationConfig, cancellationToken);
        }
    }
}
