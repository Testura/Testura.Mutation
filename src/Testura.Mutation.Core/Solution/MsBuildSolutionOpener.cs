using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Buildalyzer;
using Buildalyzer.Environment;
using Buildalyzer.Workspaces;
using Microsoft.CodeAnalysis;

namespace Testura.Mutation.Core.Solution
{
    public class MsBuildSolutionOpener : ISolutionOpener
    {
        public async Task<Microsoft.CodeAnalysis.Solution> GetSolutionAsync(string solutionPath)
        {
            StringWriter log = new StringWriter();
            AnalyzerManagerOptions options = new AnalyzerManagerOptions
            {
                LogWriter = log
            };
            var manager = new AnalyzerManager(solutionPath, options);
            using (var workspace = new AdhocWorkspace())
            {
                var projectOptions = new EnvironmentOptions { DesignTime = false };

                foreach (var projectKeyValue in manager.Projects)
                {
                    LogTo.Info($"Building {Path.GetFileNameWithoutExtension(projectKeyValue.Key)}");
                    var project = projectKeyValue.Value;

                    var results = project.Build(projectOptions);
                    if (!results.OverallSuccess)
                    {
                        LogTo.Error("Failed to build");
                        LogTo.Error(log.ToString);
                    }

                    results.Results.First().AddToWorkspace(workspace);
                }

                return await Task.FromResult(workspace.CurrentSolution);
            }
        }
    }
}