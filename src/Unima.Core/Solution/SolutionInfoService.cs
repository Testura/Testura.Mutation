using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.MSBuild;

namespace Unima.Core.Solution
{
    public class SolutionInfoService
    {
        private readonly ISolutionOpener _solutionOpener;

        public SolutionInfoService(ISolutionOpener solutionOpener)
        {
            _solutionOpener = solutionOpener;
        }

        public async Task<List<SolutionProjectInfo>> GetSolutionInfoAsync(string solutionPath)
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                var solution = await _solutionOpener.GetSolutionAsync(solutionPath);
                return solution.Projects.Select(p => new SolutionProjectInfo(p.Name, p.OutputFilePath)).ToList();
            }
        }
    }
}
