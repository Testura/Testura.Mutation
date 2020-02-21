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
using Testura.Mutation.Core.Config;

namespace Testura.Mutation.Application.Commands.Project.OpenProject
{
    public class OpenProjectCommandHandler : IRequestHandler<OpenProjectCommand, MutationConfig>
    {
        private readonly OpenProjectHandler _handler;

        public OpenProjectCommandHandler(
            OpenProjectExistHandler openProjectExistHandler,
            OpenProjectMutatorsHandler openProjectMutatorsHandler,
            OpenProjectGitFilterHandler openProjectGitFilterHandler,
            OpenProjectWorkspaceHandler openProjectWorkspaceHandler)
        {
            _handler = openProjectExistHandler;

            _handler
                .SetNext(openProjectMutatorsHandler)
                .SetNext(openProjectGitFilterHandler)
                .SetNext(openProjectWorkspaceHandler);
        }

        public async Task<MutationConfig> Handle(OpenProjectCommand command, CancellationToken cancellationToken)
        {
            var path = command.Path;
            MutationConfig applicationConfig = null;
            MutationFileConfig fileConfig = null;

            LogTo.Info($"Opening project at {command.Config?.SolutionPath ?? path}");

            try
            {
                (fileConfig, applicationConfig) = LoadConfigs(path, command.Config);

                await _handler.HandleAsync(fileConfig, applicationConfig, cancellationToken);

                LogTo.Info("Opening project finished.");
                return applicationConfig;
            }
            catch (OperationCanceledException)
            {
                LogTo.Info("Open project was cancelled by request");
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
