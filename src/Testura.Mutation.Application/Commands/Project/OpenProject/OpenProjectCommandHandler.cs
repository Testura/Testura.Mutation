using System;
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
using Testura.Mutation.Core.Creator.Filter;

namespace Testura.Mutation.Application.Commands.Project.OpenProject
{
    public class OpenProjectCommandHandler : IRequestHandler<OpenProjectCommand, MutationConfig>
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(OpenProjectCommandHandler));

        private readonly OpenProjectSolutionHandler _solutionHandler;
        private readonly OpenProjectMutatorsHandler _mutatorsHandler;
        private readonly OpenProjectGitFilterHandler _gitFilterHandler;
        private readonly OpenProjectWorkspaceHandler _workspaceHandler;
        private readonly OpenProjectBaselineHandler _baselineHandler;

        public OpenProjectCommandHandler(
            OpenProjectSolutionHandler solutionHandler,
            OpenProjectMutatorsHandler mutatorsHandler,
            OpenProjectGitFilterHandler gitFilterHandler,
            OpenProjectWorkspaceHandler workspaceHandler,
            OpenProjectBaselineHandler baselineHandler)
        {
            _solutionHandler = solutionHandler;
            _mutatorsHandler = mutatorsHandler;
            _gitFilterHandler = gitFilterHandler;
            _workspaceHandler = workspaceHandler;
            _baselineHandler = baselineHandler;
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

                // Solution and mutators
                applicationConfig.Solution = await _solutionHandler.OpenSolutionAsync(fileConfig.SolutionPath, fileConfig.BuildConfiguration, cancellationToken);
                applicationConfig.Mutators = _mutatorsHandler.InitializeMutators(fileConfig.Mutators, cancellationToken);

                // Filter
                applicationConfig.Filter.FilterItems.AddRange(_gitFilterHandler.CreateGitFilterItems(fileConfig.SolutionPath, fileConfig.Git, cancellationToken));

                // Projects
                applicationConfig.MutationProjects = _workspaceHandler.CreateMutationProjects(fileConfig, applicationConfig.Solution, cancellationToken);
                applicationConfig.TestProjects = _workspaceHandler.CreateTestProjects(fileConfig, applicationConfig.Solution, cancellationToken);

                // Baseline
                applicationConfig.BaselineInfos = await _baselineHandler.RunBaselineAsync(applicationConfig, fileConfig.CreateBaseline, cancellationToken);

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
                Filter = fileConfig.Filter ?? new MutationDocumentFilter(),
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
