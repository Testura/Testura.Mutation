using System.Threading.Tasks;
using Testura.Mutation.Core.Config;

namespace Testura.Mutation.Core.Solution
{
    public interface ISolutionOpener
    {
        Task<Microsoft.CodeAnalysis.Solution> GetSolutionAsync(TesturaMutationConfig config);

        Task<Microsoft.CodeAnalysis.Solution> GetSolutionAsync(string solutionPath);
    }
}
