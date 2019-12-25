using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Testura.Mutation.Core.Config;
using Testura.Mutation.Core.Exceptions;

namespace Testura.Mutation.Core.Solution
{
    public class MsBuildSolutionOpener : ISolutionOpener
    {
        public async Task<Microsoft.CodeAnalysis.Solution> GetSolutionAsync(TesturaMutationConfig config)
        {
            using (var workspace = MSBuildWorkspace.Create(config.TargetFramework.CreateProperties()))
            {
                LogTo.Info("Opening solution..");

                var solution = await workspace.OpenSolutionAsync(config.SolutionPath);

                if (workspace.Diagnostics.Any(w => w.Kind == WorkspaceDiagnosticKind.Failure && ContainsProjectName(w.Message, config.MutationProjects, config.TestProjects.Select(t => t.Project).ToList())))
                {
                    foreach (var workspaceDiagnostic in workspace.Diagnostics.Where(d => d.Kind == WorkspaceDiagnosticKind.Failure))
                    {
                        LogTo.Error($"Workspace error: {workspaceDiagnostic.Message}");
                    }

                    throw new ProjectSetUpException("Failed to open solution. View log for details.");
                }

                return solution;
            }
        }

        public async Task<Microsoft.CodeAnalysis.Solution> GetSolutionAsync(string solutionPath)
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                return await workspace.OpenSolutionAsync(solutionPath);
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
    }
}
