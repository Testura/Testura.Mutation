using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Log4Net;
using MediatR;
using Newtonsoft.Json;
using Testura.Mutation.Application.Commands.Project.OpenProject.Handlers;
using Testura.Mutation.Application.Exceptions;
using Testura.Mutation.Application.Models;
using Testura.Mutation.Core.Baseline;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Creator.Filter;
using Testura.Mutation.Core.Git;
using Testura.Mutation.Core.Solution;

namespace Testura.Mutation.Application.Commands.Project.OpenProject
{
    public class OpenProjectCommandHandler : IRequestHandler<OpenProjectCommand, MutationConfig>
    {
        private readonly BaselineCreator _baselineCreator;
        private readonly IGitCloner _gitCloner;
        private readonly MutationDocumentFilterItemGitDiffCreator _diffCreator;
        private readonly ISolutionBuilder _solutionBuilder;
        private readonly ISolutionOpener _solutionOpener;

        public OpenProjectCommandHandler(
            BaselineCreator baselineCreator,
            IGitCloner gitCloner,
            MutationDocumentFilterItemGitDiffCreator diffCreator,
            ISolutionBuilder solutionBuilder,
            ISolutionOpener solutionOpener)
        {
            _baselineCreator = baselineCreator;
            _gitCloner = gitCloner;
            _diffCreator = diffCreator;
            _solutionBuilder = solutionBuilder;
            _solutionOpener = solutionOpener;
        }

        public async Task<MutationConfig> Handle(OpenProjectCommand command, CancellationToken cancellationToken)
        {
            var path = command.Path;

            LogTo.Info($"Opening project at {command.Config?.SolutionPath ?? path}");

            try
            {
                var (fileConfig, applicationConfig) = LoadConfigs(path, command.Config);

                var handler = new OpenProjectExistHandler(_gitCloner);

                handler
                    .SetNext(new OpenProjectBuildHandler(_solutionBuilder))
                    .SetNext(new OpenProjectMutatorsHandler())
                    .SetNext(new OpenProjectGitFilterHandler(_diffCreator))
                    .SetNext(new OpenProjectWorkspaceHandler(_baselineCreator, _solutionOpener));

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

        private (MutationFileConfig fileConfig, MutationConfig applicationConfig) LoadConfigs(
            string path,
            MutationFileConfig fileConfig)
        {
            if (fileConfig == null)
            {
                var fileContent = File.ReadAllText(path);

                LogTo.Info($"Loading configuration: {fileContent}");

                fileConfig = JsonConvert.DeserializeObject<MutationFileConfig>(fileContent);
            }

            var config = new MutationConfig
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
