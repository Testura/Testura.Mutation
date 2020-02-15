using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace Testura.Mutation.Core.Solution
{
    public class MsBuildSolutionOpener : ISolutionOpener
    {
        public async Task<Microsoft.CodeAnalysis.Solution> GetSolutionAsync(string solutionPath, string configuration, bool buildSolution)
        {
            var visualStudioInstances = MSBuildLocator.QueryVisualStudioInstances().ToArray();
            var instance = visualStudioInstances.FirstOrDefault(vi => vi.Version == visualStudioInstances.Max(v => v.Version));

            MSBuildLocator.RegisterInstance(instance);

            LogTo.Info($"Using MSBuild at '{instance.MSBuildPath}' to load projects.");

            using (var workspace = MSBuildWorkspace.Create(new Dictionary<string, string> { ["Configuration"] = string.IsNullOrEmpty(configuration) ? "debug" : configuration }))
            {
                // Print message for WorkspaceFailed event to help diagnosing project load failures.
                workspace.WorkspaceFailed += (o, e) => LogTo.Warn(e.Diagnostic.Message);

                // Attach progress reporter so we print projects as they are loaded.
                var solution = await workspace.OpenSolutionAsync(solutionPath, new ConsoleProgressReporter());
                return solution;
            }
        }

        private class ConsoleProgressReporter : IProgress<ProjectLoadProgress>
        {
            public void Report(ProjectLoadProgress loadProgress)
            {
                var projectDisplay = Path.GetFileName(loadProgress.FilePath);
                if (loadProgress.TargetFramework != null)
                {
                    projectDisplay += $" ({loadProgress.TargetFramework})";
                }

                LogTo.Info($"{loadProgress.Operation,-15} {loadProgress.ElapsedTime,-15:m\\:ss\\.fffffff} {projectDisplay}");
            }
        }
    }
}