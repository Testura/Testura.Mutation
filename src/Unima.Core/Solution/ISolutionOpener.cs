using System.Threading.Tasks;
using Unima.Core.Config;

namespace Unima.Core.Solution
{
    public interface ISolutionOpener
    {
        Task<Microsoft.CodeAnalysis.Solution> GetSolutionAsync(UnimaConfig config);

        Task<Microsoft.CodeAnalysis.Solution> GetSolutionAsync(string solutionPath);
    }
}
