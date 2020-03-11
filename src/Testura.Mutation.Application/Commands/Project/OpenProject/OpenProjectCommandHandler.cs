﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using log4net;
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
        private static readonly ILog Log = LogManager.GetLogger(typeof(OpenProjectCommandHandler));

        private readonly OpenProjectSolutionExistHandler _solutionExistHandler;
        private readonly OpenProjectMutatorsHandler _mutatorsHandler;
        private readonly OpenProjectGitFilterHandler _gitFilterHandler;
        private readonly OpenProjectWorkspaceHandler _workspaceHandler;

        public OpenProjectCommandHandler(
            OpenProjectSolutionExistHandler solutionExistHandler,
            OpenProjectMutatorsHandler mutatorsHandler,
            OpenProjectGitFilterHandler gitFilterHandler,
            OpenProjectWorkspaceHandler workspaceHandler)
        {
            _solutionExistHandler = solutionExistHandler;
            _mutatorsHandler = mutatorsHandler;
            _gitFilterHandler = gitFilterHandler;
            _workspaceHandler = workspaceHandler;
        }

        public async Task<MutationConfig> Handle(OpenProjectCommand command, CancellationToken cancellationToken)
        {
            var path = command.Path;
            MutationConfig applicationConfig = null;
            MutationFileConfig fileConfig = null;

            Log.Info($"Opening project at {command.Config?.SolutionPath ?? path}");

            try
            {
                (fileConfig, applicationConfig) = LoadConfigs(path, command.Config);

                _solutionExistHandler.VerifySolutionExist(fileConfig.SolutionPath, cancellationToken);
                applicationConfig.Mutators = _mutatorsHandler.InitializeMutators(fileConfig.Mutators, cancellationToken);

                _gitFilterHandler.InitializeGitFilter(fileConfig.SolutionPath, fileConfig.Git, applicationConfig, cancellationToken);

                await _workspaceHandler.InitializeProjectAsync(fileConfig, applicationConfig, cancellationToken);

                Log.Info("Opening project finished.");
                return applicationConfig;
            }
            catch (OperationCanceledException)
            {
                Log.Info("Open project was cancelled by request");
                return applicationConfig;
            }
            catch (Exception ex)
            {
                Log.Error("Failed to open project");
                Log.Error(ex.Message);
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

                Log.Info($"Loading configuration: {fileContent}");

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
            };

            return (fileConfig, config);
        }
    }
}
