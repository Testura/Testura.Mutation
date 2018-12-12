using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Cama.Application.Exceptions;
using Cama.Application.Models;
using Cama.Core;
using Cama.Core.Baseline;
using Cama.Core.Exceptions;
using Cama.Core.Solution;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Newtonsoft.Json;

namespace Cama.Application.Commands.Project.OpenProject
{
    public class OpenProjectCommandHandler : IRequestHandler<OpenProjectCommand, CamaConfig>
    {
        private readonly BaselineCreator _baselineCreator;

        public OpenProjectCommandHandler(BaselineCreator baselineCreator)
        {
            _baselineCreator = baselineCreator;
        }

        public async Task<CamaConfig> Handle(OpenProjectCommand command, CancellationToken cancellationToken)
        {
            var path = command.Path;

            LogTo.Info($"Opening project at {path}");

            try
            {
                var fileContent = File.ReadAllText(path);

                LogTo.Info($"Loading configuration: {fileContent}");

                var fileConfig = JsonConvert.DeserializeObject<CamaFileConfig>(fileContent);
                var config = new CamaConfig
                {
                    SolutionPath = fileConfig.SolutionPath,
                    Filter = fileConfig.Filter ?? new List<string>(),
                    NumberOfTestRunInstances = fileConfig.NumberOfTestRunInstances,
                    BuildConfiguration = fileConfig.BuildConfiguration,
                    MaxTestTimeMin = fileConfig.MaxTestTimeMin,
                    TestRunner = fileConfig.TestRunner
                };

                using (var workspace = MSBuildWorkspace.Create())
                {
                    LogTo.Info("Opening solution..");

                    if (!File.Exists(config.SolutionPath))
                    {
                        throw new FileNotFoundException("Could not find solution", config.SolutionPath);
                    }

                    var solution = await workspace.OpenSolutionAsync(config.SolutionPath);

                    if (workspace.Diagnostics.Any(w => w.Kind == WorkspaceDiagnosticKind.Failure && ContainsProjectName(w.Message, config.MutationProjects, config.TestProjects)))
                    {
                        foreach (var workspaceDiagnostic in workspace.Diagnostics.Where(d => d.Kind == WorkspaceDiagnosticKind.Failure))
                        {
                            LogTo.Error($"Workspace error: {workspaceDiagnostic.Message}");
                        }

                        throw new ProjectSetUpException("Failed to open solution. View log for details.");
                    }

                    InitializeTestProjects(fileConfig, config, solution);
                    InitializeMutationProjects(fileConfig, config, solution);

                    if (command.CreateBaseline)
                    {
                       config.BaselineInfos = new List<BaselineInfo>(await _baselineCreator.CreateBaselineAsync(config, solution));
                    }

                    workspace.CloseSolution();
                    LogTo.Info("Opening project finished.");
                }

                return config;
            }
            catch (Exception ex)
            {
                LogTo.ErrorException("Failed to open project", ex);
                throw new OpenProjectException("Failed to open project", ex);
            }
        }

        private bool ContainsProjectName(string message, IList<SolutionProjectInfo> mutationProjects, IList<SolutionProjectInfo> testProjects)
        {
            foreach (var configMutationProject in mutationProjects)
            {
                if (message.Contains(configMutationProject.Name))
                {
                    return true;
                }
            }

            foreach (var configMutationProject in testProjects)
            {
                if (message.Contains(Path.GetFileNameWithoutExtension(configMutationProject.OutputFileName)))
                {
                    return true;
                }
            }

            return false;
        }

        private void InitializeMutationProjects(CamaFileConfig fileConfig, CamaConfig config, Microsoft.CodeAnalysis.Solution solution)
        {
            if (fileConfig.IgnoredMutationProjects == null)
            {
                fileConfig.IgnoredMutationProjects = new List<string>();
            }

            LogTo.Info("Setting up mutation projects.");
            foreach (var solutionProject in solution.Projects)
            {
                if (fileConfig.IgnoredMutationProjects.Contains(solutionProject.Name) || fileConfig.TestProjects.Contains(solutionProject.Name))
                {
                    continue;
                }

                LogTo.Info($"Grabbing output info for {solutionProject.Name}.");

                config.MutationProjects.Add(new SolutionProjectInfo(solutionProject.Name, solutionProject.OutputFilePath));
            }
        }

        private void InitializeTestProjects(CamaFileConfig fileConfig, CamaConfig config, Microsoft.CodeAnalysis.Solution solution)
        {
            if (fileConfig.TestProjects == null || !fileConfig.TestProjects.Any())
            {
                throw new ProjectSetUpException("Test project list is null or empty");
            }

            LogTo.Info("Setting up test projects.");
            foreach (var testProjectName in fileConfig.TestProjects)
            {
                var testProject = solution.Projects.FirstOrDefault(p => p.Name == testProjectName);

                if (testProject == null)
                {
                    throw new ProjectSetUpException($"Could not find any project with the name {testProjectName} in the solution.");
                }

                LogTo.Info($"Found the test project {testProjectName}. Grabbing output info.");

                var testProjectOutput = testProject.OutputFilePath;

                // We replace in path just to make sure...
                if (config.BuildConfiguration != null && config.BuildConfiguration.Equals("release", StringComparison.InvariantCultureIgnoreCase))
                {
                    testProjectOutput = testProjectOutput.Replace("/debug/", "/release/");
                }
                else
                {
                    testProjectOutput = testProjectOutput.Replace("/release/", "/debug/");
                }

                LogTo.Info($"Wanted build configuration is \"{config.BuildConfiguration}\". Setting test project output to \"{testProjectOutput}\"");
                config.TestProjects.Add(new SolutionProjectInfo(testProject.Name, testProjectOutput));
            }
        }
    }
}
