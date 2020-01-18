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
        public async Task<Microsoft.CodeAnalysis.Solution> GetSolutionAsync(MutationConfig config)
        {
            using (var workspace = MSBuildWorkspace.Create(config.TargetFramework.CreateProperties()))
            {
                LogTo.Info("Opening solution..");

                var solution = await workspace.OpenSolutionAsync(config.SolutionPath);

                if (workspace.Diagnostics.Any(w => w.Kind == WorkspaceDiagnosticKind.Failure && ContainsProjectName(w.Message, config.MutationProjects, config.TestProjects)))
                {
                    LogTo.Error("Failed to open solution because of diagnostic errors.");

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

        private bool ContainsProjectName(string message, IList<MutationProject> mutationProjects, IList<TestProject> testProjects)
        {
            foreach (var mutationProject in mutationProjects)
            {
                if (message.Contains(Path.GetFileName(mutationProject.Project.FilePath ?? string.Empty)))
                {
                    return true;
                }
            }

            foreach (var configMutationProject in testProjects)
            {
                if (message.Contains(Path.GetFileName(configMutationProject.Project.FilePath ?? string.Empty)))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
