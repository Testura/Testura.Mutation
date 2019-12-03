using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Log4Net;
using MediatR;
using Newtonsoft.Json;
using Unima.Application.Commands.Project.OpenProject.Handlers;
using Unima.Application.Exceptions;
using Unima.Application.Models;
using Unima.Core.Baseline;
using Unima.Core.Config;
using Unima.Core.Creator.Filter;
using Unima.Core.Git;
using Unima.Core.Solution;

namespace Unima.Application.Commands.Project.OpenProject
{
    public class OpenProjectCommandHandler : IRequestHandler<OpenProjectCommand, UnimaConfig>
    {
        private readonly BaselineCreator _baselineCreator;
        private readonly IGitCloner _gitCloner;
        private readonly MutationDocumentFilterItemGitDiffCreator _diffCreator;
        private readonly ISolutionBuilder _solutionBuilder;

        public OpenProjectCommandHandler(
            BaselineCreator baselineCreator,
            IGitCloner gitCloner,
            MutationDocumentFilterItemGitDiffCreator diffCreator,
            ISolutionBuilder solutionBuilder)
        {
            _baselineCreator = baselineCreator;
            _gitCloner = gitCloner;
            _diffCreator = diffCreator;
            _solutionBuilder = solutionBuilder;
        }

        public async Task<UnimaConfig> Handle(OpenProjectCommand command, CancellationToken cancellationToken)
        {
            var path = command.Path;

            LogTo.Info($"Opening project at {path}");

            try
            {
                var (fileConfig, applicationConfig) = LoadConfigs(path);

                var handler = new OpenProjectExistHandler(_gitCloner);

                    handler
                    .SetNext(new OpenProjectBuildHandler(_solutionBuilder))
                    .SetNext(new OpenProjectMutatorsHandler())
                    .SetNext(new OpenProjectGitFilterHandler(_diffCreator))
                    .SetNext(new OpenProjectWorkspaceHandler(_baselineCreator));

                await handler.HandleAsync(fileConfig, applicationConfig);

                LogTo.Info("Opening project finished.");

                return applicationConfig;
            }
            catch (Exception ex)
            {
                LogTo.ErrorException("Failed to open project", ex);
                throw new OpenProjectException("Failed to open project", ex);
            }
        }

        private (UnimaFileConfig fileConfig, UnimaConfig applicationConfig) LoadConfigs(string path)
        {
            var fileContent = File.ReadAllText(path);

            LogTo.Info($"Loading configuration: {fileContent}");

            var fileConfig = JsonConvert.DeserializeObject<UnimaFileConfig>(fileContent);

            var config = new UnimaConfig
            {
                SolutionPath = fileConfig.SolutionPath,
                Filter = fileConfig.Filter,
                NumberOfTestRunInstances = fileConfig.NumberOfTestRunInstances,
                BuildConfiguration = fileConfig.BuildConfiguration,
                MaxTestTimeMin = fileConfig.MaxTestTimeMin,
                DotNetPath = fileConfig.DotNetPath,
                MutationRunLoggers = fileConfig.MutationRunLoggers,
                TargetFramework = fileConfig.TargetFramework ?? new TargetFramework()
            };

            return (fileConfig, config);
        }
    }
}
