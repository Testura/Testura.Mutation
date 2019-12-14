using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServices;
using Unima.Core.Config;

namespace Unima.Core.Solution
{
    public class VisualStudioSolutionOpener : ISolutionOpener
    {
        private readonly VisualStudioWorkspace _visualStudioWorkspace;

        public VisualStudioSolutionOpener(VisualStudioWorkspace visualStudioWorkspace)
        {
            _visualStudioWorkspace = visualStudioWorkspace;
        }

        public Task<Microsoft.CodeAnalysis.Solution> GetSolutionAsync(UnimaConfig config)
        {
            return Task.FromResult(_visualStudioWorkspace.CurrentSolution);
        }

        public Task<Microsoft.CodeAnalysis.Solution> GetSolutionAsync(string solutionPath)
        {
            return Task.FromResult(_visualStudioWorkspace.CurrentSolution);
        }
    }
}
