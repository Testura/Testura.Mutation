using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Anotar.Log4Net;
using Buildalyzer;
using Buildalyzer.Environment;
using Microsoft.CodeAnalysis;
using Testura.Mutation.Core.Extensions;

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

                foreach (var projectKeyValue in manager.Projects)
                {
                    LogTo.Info($"Building {Path.GetFileNameWithoutExtension(projectKeyValue.Key)}");
                    var project = projectKeyValue.Value;
                    var results = project.Build(environmentOptions);
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