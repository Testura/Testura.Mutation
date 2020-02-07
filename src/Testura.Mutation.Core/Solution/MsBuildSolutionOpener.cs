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
            var log = new StringWriter();
            var analyzerOptions = new AnalyzerManagerOptions
            {
                LogWriter = log
            };

            var manager = new AnalyzerManager(solutionPath, analyzerOptions);
            using (var workspace = new AdhocWorkspace())
            {
                var environmentOptions = new EnvironmentOptions { DesignTime = false };
                environmentOptions.TargetsToBuild.Remove("Clean");

                foreach (var projectKeyValue in manager.Projects)
                {
                    var project = projectKeyValue.Value;
                    var results = project.Build(environmentOptions);

                    results.Results.First().AddToWorkspace(workspace);
                }

                return await Task.FromResult(workspace.CurrentSolution);
            }
        }
    }
}