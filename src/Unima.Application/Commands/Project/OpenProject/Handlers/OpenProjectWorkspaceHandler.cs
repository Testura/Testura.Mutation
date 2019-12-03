using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Unima.Application.Exceptions;
using Unima.Application.Models;
using Unima.Core.Baseline;
using Unima.Core.Config;
using Unima.Core.Exceptions;
using Unima.Core.Solution;

namespace Unima.Application.Commands.Project.OpenProject.Handlers
{
    public class OpenProjectWorkspaceHandler : OpenProjectHandler
    {
        private readonly BaselineCreator _baselineCreator;

        public OpenProjectWorkspaceHandler(BaselineCreator baselineCreator)
        {
            _baselineCreator = baselineCreator;
        }

        public override async Task HandleAsync(UnimaFileConfig fileConfig, UnimaConfig applicationConfig)
        {
            using (var workspace = MSBuildWorkspace.Create(applicationConfig.TargetFramework.CreateProperties()))
            {
                LogTo.Info("Opening solution..");

                var solution = await workspace.OpenSolutionAsync(fileConfig.SolutionPath);

                if (workspace.Diagnostics.Any(w => w.Kind == WorkspaceDiagnosticKind.Failure && ContainsProjectName(w.Message, applicationConfig.MutationProjects, applicationConfig.TestProjects.Select(t => t.Project).ToList())))
                {
                    foreach (var workspaceDiagnostic in workspace.Diagnostics.Where(d => d.Kind == WorkspaceDiagnosticKind.Failure))
                    {
                        LogTo.Error($"Workspace error: {workspaceDiagnostic.Message}");
                    }

                    throw new ProjectSetUpException("Failed to open solution. View log for details.");
                }

                InitializeTestProjects(fileConfig, applicationConfig, solution);
                InitializeMutationProjects(fileConfig, applicationConfig, solution);

                if (fileConfig.CreateBaseline)
                {
                    applicationConfig.BaselineInfos = new List<BaselineInfo>(await _baselineCreator.CreateBaselineAsync(applicationConfig, solution));
                }

                workspace.CloseSolution();
            }

            await base.HandleAsync(fileConfig, applicationConfig);
        }

        private Dictionary<string, string> CreateProperties(UnimaFileConfig fileConfig)
        {
            var props = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(fileConfig.TargetFramework?.Name))
            {
                LogTo.Info($"Found a target framework in config: {fileConfig.TargetFramework.Name}. Adding it to properties");
                props.Add("TargetFramework", fileConfig.TargetFramework.Name);
            }

            return props;
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


        private void InitializeMutationProjects(UnimaFileConfig fileConfig, UnimaConfig config, Microsoft.CodeAnalysis.Solution solution)
        {
            if (fileConfig.IgnoredProjects == null)
            {
                fileConfig.IgnoredProjects = new List<string>();
            }

            LogTo.Info("Setting up mutation projects.");
            foreach (var solutionProject in solution.Projects)
            {
                if (
                    IsIgnored(solutionProject.Name, fileConfig.IgnoredProjects) ||
                    IsTestProject(solutionProject.Name, fileConfig.TestProjects) ||
                    WeTargetSpecificFrameworkThatThisProjectDontSupport(solutionProject.FilePath, fileConfig.TargetFramework))
                {
                    continue;
                }

                LogTo.Info($"Grabbing output info for {solutionProject.Name}.");

                config.MutationProjects.Add(new SolutionProjectInfo(solutionProject.Name, UpdateOutputPathWithBuildConfiguration(solutionProject.OutputFilePath, config.BuildConfiguration)));
            }
        }

        private bool WeTargetSpecificFrameworkThatThisProjectDontSupport(string projectFilePath, TargetFramework targetFramework)
        {
            if (targetFramework == null ||
                string.IsNullOrEmpty(targetFramework.Name) ||
                targetFramework.IgnoreProjectsWithWrongTargetFramework == false)
            {
                return false;
            }

            var regex = new Regex("<TargetFramework>(.*)</TargetFramework>");
            var o = regex.Match(File.ReadAllText(projectFilePath));
            var content = o.Groups[1].ToString();

            return !content.ToLower().Contains(targetFramework.Name.ToLower());
        }

        private void InitializeTestProjects(UnimaFileConfig fileConfig, UnimaConfig config, Solution solution)
        {
            if (fileConfig.TestProjects == null || !fileConfig.TestProjects.Any())
            {
                throw new ProjectSetUpException("Test project list is null or empty");
            }

            LogTo.Info("Setting up test projects.");
            foreach (var testProjectName in fileConfig.TestProjects)
            {
                var testProjects = solution.Projects.Where(p => Regex.IsMatch(p.Name, FormattedProjectName(testProjectName), RegexOptions.IgnoreCase));

                if (!testProjects.Any())
                {
                    throw new ProjectSetUpException($"Could not find any project with the name {testProjectName} in the solution. List of project names: {string.Join(", ", solution.Projects.Select(p => p.Name))}");
                }

                foreach (var testProject in testProjects)
                {
                    LogTo.Info($"Found the test project {testProject.Name}. Grabbing output info.");

                    if (IsIgnored(testProject.Name, fileConfig.IgnoredProjects))
                    {
                        LogTo.Info("But it was ignored. So skipping");
                        continue;
                    }

                    if (WeTargetSpecificFrameworkThatThisProjectDontSupport(testProject.FilePath,
                        fileConfig.TargetFramework))
                    {
                        LogTo.Info("Project does not target expected framework");
                        continue;
                    }

                    var testProjectOutput = UpdateOutputPathWithBuildConfiguration(testProject.OutputFilePath, config.BuildConfiguration);
                    LogTo.Info($"Wanted build configuration is \"{config.BuildConfiguration}\". Setting test project output to \"{testProjectOutput}\"");
                    config.TestProjects.Add(new TestProject { Project = new SolutionProjectInfo(testProject.Name, testProjectOutput), TestRunner = GetTestRunner(testProject, fileConfig.TestRunner) });
                }
            }
        }

        private string GetTestRunner(Microsoft.CodeAnalysis.Project testProject, string fileConfigTestRunner)
        {
            //This is a bit hackish but it's until I fix so you can specify test runner in file config. 
            LogTo.Info($"Looking for test runner for {testProject.Name}..");
            if (!string.IsNullOrEmpty(fileConfigTestRunner))
            {
                LogTo.Info($"..found {fileConfigTestRunner} in config.");
                return fileConfigTestRunner;
            }

            if (testProject.ParseOptions.PreprocessorSymbolNames.Any(p => p.ToUpper().Contains("NETCOREAPP")))
            {
                LogTo.Info("..found .core in symbol names so will use dotnet.");
                return "dotnet";
            }

            if (testProject.MetadataReferences.Any(m => m.Display.ToLower().Contains("nunit")))
            {
                LogTo.Info("..found nunit in references so will use that.");
                return "nunit";
            }

            if (testProject.MetadataReferences.Any(m => m.Display.ToLower().Contains("xunit")))
            {
                LogTo.Info("..found xunit in references so will use that.");
                return "xunit";
            }

            throw new OpenProjectException($"Could not determine test runner for {testProject.Name}. Please specify test runner in config");
        }

        private bool IsIgnored(string projectName, IList<string> ignoredProjects)
        {
            if (ignoredProjects == null)
            {
                return false;
            }

            return ignoredProjects.Any(ignoredProject => Regex.IsMatch(projectName, FormattedProjectName(ignoredProject), RegexOptions.IgnoreCase));
        }

        private bool IsTestProject(string projectName, IList<string> testProjects)
        {
            if (testProjects == null)
            {
                return false;
            }

            return testProjects.Any(testProject => Regex.IsMatch(projectName, FormattedProjectName(testProject), RegexOptions.IgnoreCase));
        }

        private string UpdateOutputPathWithBuildConfiguration(string path, string buildConfiguration)
        {
            // We replace in path just to make sure...
            if (buildConfiguration != null && buildConfiguration.Equals("release", StringComparison.InvariantCultureIgnoreCase))
            {
                return path.Replace("\\Debug\\", "\\Release\\");
            }

            return path.Replace("\\Release\\", "\\debug\\");
        }

        private string FormattedProjectName(string projectName)
        {
            return "^" + Regex.Escape(projectName).Replace("\\*", ".*") + "$";
        }
    }
}
