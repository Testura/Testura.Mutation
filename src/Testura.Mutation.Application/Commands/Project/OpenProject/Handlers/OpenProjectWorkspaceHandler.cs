using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using log4net;
using Microsoft.CodeAnalysis;
using Testura.Mutation.Application.Models;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Exceptions;
using Testura.Mutation.Core.Solution;

namespace Testura.Mutation.Application.Commands.Project.OpenProject.Handlers
{
    public class OpenProjectWorkspaceHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(OpenProjectWorkspaceHandler));

        public IList<MutationProject> CreateMutationProjects(MutationFileConfig fileConfig, Solution solution, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var mutationProjects = new List<MutationProject>();

            Log.Info("Setting up mutation projects.");
            foreach (var solutionProject in solution.Projects)
            {
                if (
                    IsIgnored(solutionProject.Name, fileConfig.IgnoredProjects) ||
                    IsTestProject(solutionProject.Name, fileConfig.TestProjects))
                {
                    continue;
                }

                Log.Info($"Grabbing output info for {solutionProject.Name}.");

                mutationProjects.Add(new MutationProject
                {
                    Project = new SolutionProjectInfo(solutionProject.Name, solutionProject.FilePath, UpdateOutputPathWithBuildConfiguration(solutionProject.OutputFilePath, fileConfig.BuildConfiguration)),
                    MappedTestProjects = GetMappedProjects(solutionProject.Name, fileConfig.ProjectMappings)
                });
            }

            return mutationProjects;
        }

        public List<TestProject> CreateTestProjects(MutationFileConfig fileConfig, Solution solution, CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var testProjects = new List<TestProject>();

            if (fileConfig.TestProjects == null || !fileConfig.TestProjects.Any())
            {
                Log.Error("Test project list is null or empty");
                throw new ProjectSetUpException("Test project list is null or empty");
            }

            Log.Info("Setting up test projects.");
            foreach (var testProjectName in fileConfig.TestProjects)
            {
                var matchedTestProjects = solution.Projects.Where(p => Regex.IsMatch(p.Name, FormattedProjectName(testProjectName), RegexOptions.IgnoreCase));

                if (!matchedTestProjects.Any())
                {
                    throw new ProjectSetUpException($"Could not find any project with the name {testProjectName} in the solution. List of project names: {string.Join(", ", solution.Projects.Select(p => p.Name))}");
                }

                foreach (var testProject in matchedTestProjects)
                {
                    Log.Info($"Found the test project {testProject.Name}. Grabbing output info.");

                    if (IsIgnored(testProject.Name, fileConfig.IgnoredProjects))
                    {
                        Log.Info("But it was ignored. So skipping");
                        continue;
                    }

                    if (testProjects.Any(t => t.Project.Name == testProject.Name))
                    {
                        continue;
                    }

                    var testProjectOutput = UpdateOutputPathWithBuildConfiguration(testProject.OutputFilePath, fileConfig.BuildConfiguration);
                    Log.Info($"Wanted build configuration is \"{fileConfig.BuildConfiguration}\". Setting test project output to \"{testProjectOutput}\"");
                    testProjects.Add(new TestProject
                    {
                        Project = new SolutionProjectInfo(testProject.Name, testProject.FilePath, testProjectOutput),
                        TestRunner = GetTestRunner(testProject, fileConfig.TestRunner)
                    });
                }
            }

            return testProjects;
        }

        private string GetTestRunner(Microsoft.CodeAnalysis.Project testProject, string fileConfigTestRunner)
        {
            var testRunner = fileConfigTestRunner ?? "DotNet";

            Log.Info($"Test runner: {testRunner}");
            return testRunner;
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
