using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.MSBuild;

namespace Unima.Core.Solution
{
    public class SolutionInfoService
    {
        public async Task<List<SolutionProjectInfo>> GetSolutionInfoAsync(string solutionPath)
        {
            var workspace = MSBuildWorkspace.Create();
            var solution = await workspace.OpenSolutionAsync(solutionPath);

            return solution.Projects.Select(p => new SolutionProjectInfo(p.Name, p.OutputFilePath)).ToList();
        }
    }
}
