using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServices;

namespace Testura.Mutation.Core.Solution
{
    public class VisualStudioSolutionOpener : ISolutionOpener
    {
        private readonly VisualStudioWorkspace _visualStudioWorkspace;
        private readonly ISolutionBuilder _solutionBuilder;

        public VisualStudioSolutionOpener(VisualStudioWorkspace visualStudioWorkspace, ISolutionBuilder solutionBuilder)
        {
            _visualStudioWorkspace = visualStudioWorkspace;
            _solutionBuilder = solutionBuilder;
        }

        public Task<Microsoft.CodeAnalysis.Solution> GetSolutionAsync(string solutionPath)
        {
            _solutionBuilder.BuildSolution(null);
            return Task.FromResult(_visualStudioWorkspace.CurrentSolution);
        }
    }
}
