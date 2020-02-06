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
            var workspace = new AdhocWorkspace();
            foreach (var projectKeyValue in manager.Projects)
            {
                LogTo.Info($"Building {Path.GetFileNameWithoutExtension(projectKeyValue.Key)}");
                var project = projectKeyValue.Value;

                EnvironmentOptions options2 = new EnvironmentOptions();
                options2.DesignTime = false;

                var results = project.Build(options2);

                results.Results.First().AddToWorkspace(workspace);
            }

            var o = log.ToString();

            return await Task.FromResult(workspace.CurrentSolution);
        }
    }
}