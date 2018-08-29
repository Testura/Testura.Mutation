using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Cama.Core.Exceptions;
using Cama.Core.Models.Project;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Newtonsoft.Json;

namespace Cama.Core.Services.Project
{
    public class ProjectService : ICreateProjectService, IOpenProjectService
    {
        public void CreateProject(string savePath, CamaFileConfig config)
        {
            LogTo.Info("Creating project file");
            File.WriteAllText(savePath, JsonConvert.SerializeObject(config));
        }

        public async Task<CamaConfig> OpenProjectAsync(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentException("Invalid solution path", nameof(path));
            }

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"Could not find config", path);
            }

            LogTo.Info($"Opening project at {path}");

            var fileConfig = JsonConvert.DeserializeObject<CamaFileConfig>(File.ReadAllText(path));
            var config = new CamaConfig { SolutionPath = fileConfig.SolutionPath, Filter = fileConfig.Filter ?? new List<string>() };

            MSBuildLocator.RegisterDefaults();
            var props = new Dictionary<string, string> { ["Platform"] = "AnyCPU" };

            using (var workspace = MSBuildWorkspace.Create(props))
            {
                LogTo.Info("Opening solution..");

                if (!File.Exists(config.SolutionPath))
                {
                    throw new FileNotFoundException("Could not find solution", config.SolutionPath);
                }

                var solution = await workspace.OpenSolutionAsync(config.SolutionPath);

                if (workspace.Diagnostics.Any(w => w.Kind == WorkspaceDiagnosticKind.Failure))
                {
                    foreach (var workspaceDiagnostic in workspace.Diagnostics.Where(d => d.Kind == WorkspaceDiagnosticKind.Failure))
                    {
                        LogTo.Error($"Workspace error: {workspaceDiagnostic.Message}");
                    }

                    throw new ProjectSetUpException("Failed to open solution. View log for details.");
                }

                InitializeTestProjects(fileConfig, config, solution);
                InitializeMutationProjects(fileConfig, config, solution);

                workspace.CloseSolution();
                LogTo.Info("Opening project finished.");
            }

            return config;
        }

        private static void InitializeMutationProjects(CamaFileConfig fileConfig, CamaConfig config, Microsoft.CodeAnalysis.Solution solution)
        {
            LogTo.Info("Setting up mutation projects.");
            foreach (var localConfigMutationProjectName in fileConfig.MutationProjects)
            {
                var mutationProject = solution.Projects.FirstOrDefault(p => p.Name == localConfigMutationProjectName);

                if (mutationProject == null)
                {
                    throw new ProjectSetUpException($"Could not find any project with the name {localConfigMutationProjectName} in the solution.");
                }

                config.MutationProjects.Add(new MutationProjectInfo
                {
                    MutationProjectName = localConfigMutationProjectName,
                    MutationProjectOutputFileName = Path.GetFileName(mutationProject.OutputFilePath)
                });
            }
        }

        private static void InitializeTestProjects(CamaFileConfig fileConfig, CamaConfig config, Microsoft.CodeAnalysis.Solution solution)
        {
            LogTo.Info("Setting up test projects.");
            foreach (var testProjectName in fileConfig.TestProjects)
            {
                var testProject = solution.Projects.FirstOrDefault(p => p.Name == testProjectName);

                if (testProject == null)
                {
                    throw new ProjectSetUpException($"Could not find any project with the name {testProjectName} in the solution.");
                }

                var testProjectOutput = testProject.OutputFilePath;
                config.TestProjects.Add(new TestProjectInfo
                {
                    TestProjectOutputPath = Path.GetDirectoryName(testProjectOutput),
                    TestProjectOutputFileName = Path.GetFileName(testProjectOutput)
                });
            }
        }
    }
}
