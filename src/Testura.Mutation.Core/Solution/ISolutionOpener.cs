using System.Threading.Tasks;

namespace Testura.Mutation.Core.Solution
{
    public interface ISolutionOpener
    {
        Task<Microsoft.CodeAnalysis.Solution> GetSolutionAsync(string solutionPath, string configuration, bool buildSolution);
    }
}