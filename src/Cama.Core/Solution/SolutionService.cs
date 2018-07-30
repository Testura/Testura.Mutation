using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cama.Core.Models.Solution;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;

namespace Cama.Core.Solution
{
    public class SolutionService
    {
        public async Task<List<SolutionProjectInfo>> GetSolutionInfoAsync(string solutionPath)
        {
            MSBuildLocator.RegisterDefaults();
            var props = new Dictionary<string, string> { ["Platform"] = "AnyCPU" };
            var workspace = MSBuildWorkspace.Create(props);
            var solution = await workspace.OpenSolutionAsync(solutionPath);

            return solution.Projects.Select(p => new SolutionProjectInfo(p.Id.Id, p.Name)).ToList();
        }
    }
}
