using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Testura.Mutation.Core.Solution;

namespace Testura.Mutation.Tests.Utils.Stubs
{
    public class TestSolutionOpener : ISolutionOpener
    {
        private readonly Solution _solution;

        public TestSolutionOpener(Solution solution)
        {
            _solution = solution;
        }

        public Task<Solution> GetSolutionAsync(string solutionPath, string configuration, bool buildSolution)
        {
            return Task.FromResult(_solution);
        }
    }
}
