using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Testura.Mutation.Application.Exceptions;
using Testura.Mutation.Application.Models;
using Testura.Mutation.Core.Baseline;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Exceptions;
using Testura.Mutation.Core.Solution;

namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers
{
    public class OpenProjectWorkspaceHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(OpenProjectWorkspaceHandler));

        private readonly BaselineCreator _baselineCreator;
        private readonly ISolutionOpener _solutionOpener;

        public OpenProjectWorkspaceHandler(BaselineCreator baselineCreator, ISolutionOpener solutionOpener)
        {
            _baselineCreator = baselineCreator;
            _solutionOpener = solutionOpener;
        }

        public async Task InitializeProjectAsync(MutationFileConfig fileConfig, MutationConfig applicationConfig, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            applicationConfig.Solution = await _solutionOpener.GetSolutionAsync(fileConfig.SolutionPath, fileConfig.BuildConfiguration, true);

            InitializeTestProjects(fileConfig, applicationConfig);
            InitializeMutationProjects(fileConfig, applicationConfig);

            if (fileConfig.CreateBaseline)
            {
                applicationConfig.BaselineInfos = new List<BaselineInfo>(await _baselineCreator.CreateBaselineAsync(applicationConfig, cancellationToken));
            }
        }

        private void InitializeMutationProjects(MutationFileConfig fileConfig, MutationConfig config)
        {
            if (fileConfig.IgnoredProjects == null)
            {
                fileConfig.IgnoredProjects = new List<string>();
            }

            Log.Info("Setting up mutation projects.");
            foreach (var solutionProject in config.Solution.Projects)
            {
                if (
                    IsIgnored(solutionProject.Name, fileConfig.IgnoredProjects) ||
                    IsTestProject(solutionProject.Name, fileConfig.TestProjects) ||
                    WeTargetSpecificFrameworkThatThisProjectDontSupport(solutionProject.FilePath, fileConfig.TargetFramework))
                {
                    continue;
                }

                Log.Info($"Grabbing output info for {solutionProject.Name}.");

                config.MutationProjects.Add(new MutationProject
                {
                    Project = new SolutionProjectInfo(solutionProject.Name, solutionProject.FilePath, UpdateOutputPathWithBuildConfiguration(solutionProject.OutputFilePath, config.BuildConfiguration)),
                    MappedTestProjects = GetMappedProjects(solutionProject.Name, fileConfig.ProjectMappings)
                });
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

        private void InitializeTestProjects(MutationFileConfig fileConfig, MutationConfig config)
        {
            if (fileConfig.TestProjects == null || !fileConfig.TestProjects.Any())
            {
                Log.Error("Test project list is null or empty");
                throw new ProjectSetUpException("Test project list is null or empty");
            }

            Log.Info("Setting up test projects.");
            foreach (var testProjectName in fileConfig.TestProjects)
            {
                var testProjects = config.Solution.Projects.Where(p => Regex.IsMatch(p.Name, FormattedProjectName(testProjectName), RegexOptions.IgnoreCase));

                if (!testProjects.Any())
                {
                    throw new ProjectSetUpException($"Could not find any project with the name {testProjectName} in the solution. List of project names: {string.Join(", ", config.Solution.Projects.Select(p => p.Name))}");
                }

                foreach (var testProject in testProjects)
                {
                    Log.Info($"Found the test project {testProject.Name}. Grabbing output info.");

                    if (IsIgnored(testProject.Name, fileConfig.IgnoredProjects))
                    {
                        Log.Info("But it was ignored. So skipping");
                        continue;
                    }

                    if (WeTargetSpecificFrameworkThatThisProjectDontSupport(
                        testProject.FilePath,
                        fileConfig.TargetFramework))
                    {
                        Log.Info("Project does not target expected framework");
                        continue;
                    }

                    if (config.TestProjects.Any(t => t.Project.Name == testProject.Name))
                    {
                        continue;
                    }

                    var testProjectOutput = UpdateOutputPathWithBuildConfiguration(testProject.OutputFilePath, config.BuildConfiguration);
                    Log.Info($"Wanted build configuration is \"{config.BuildConfiguration}\". Setting test project output to \"{testProjectOutput}\"");
                    config.TestProjects.Add(new TestProject
                    {
                        Project = new SolutionProjectInfo(testProject.Name, testProject.FilePath, testProjectOutput),
                        TestRunner = GetTestRunner(testProject, fileConfig.TestRunner)
                    });
                }
            }
        }

        private string GetTestRunner(Microsoft.CodeAnalysis.Project testProject, string fileConfigTestRunner)
        {
            // This is a bit hackish but it's until I fix so you can specify test runner in file config.
            Log.Info($"Looking for test runner for {testProject.Name}..");
            if (!string.IsNullOrEmpty(fileConfigTestRunner))
            {
                Log.Info($"..found {fileConfigTestRunner} in config.");
                return fileConfigTestRunner;
            }

            if (testProject.ParseOptions.PreprocessorSymbolNames.Any(p => p.ToUpper().Contains("NETCOREAPP")))
            {
                Log.Info("..found .core in symbol names so will use dotnet.");
                return "dotnet";
            }

            if (testProject.MetadataReferences.Any(m => m.Display.ToLower().Contains("nunit")))
            {
                Log.Info("..found nunit in references so will use that.");
                return "nunit";
            }

            if (testProject.MetadataReferences.Any(m => m.Display.ToLower().Contains("xunit")))
            {
                Log.Info("..found xunit in references so will use that.");
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

        private List<string> GetMappedProjects(string mutationProjectName, IList<ProjectMapping> projectMappings)
        {
            var projectMapping = projectMappings.FirstOrDefault(p => p.ProjectName == mutationProjectName);

            if (projectMapping == null)
            {
                return new List<string>();
            }

            return new List<string>(projectMapping.TestProjectNames);
        }
    }
}
