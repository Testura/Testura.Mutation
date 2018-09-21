using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace Cama.Core.Solution
{
    public class SolutionInfoService
    {
        public async Task<List<SolutionProjectInfo>> GetSolutionInfoAsync(string solutionPath)
        {
            MSBuildLocator.RegisterDefaults();
            var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(solutionPath);

            return solution.Projects.Select(p => new SolutionProjectInfo(p.Name, p.OutputFilePath)).ToList();
        }
    }
}
