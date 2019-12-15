using EnvDTE;
using Microsoft.VisualStudio.Threading;
using Unima.Core.Solution;

namespace Unima.VsExtension.Solution
{
    public class VisualStudioSolutionBuilder : ISolutionBuilder
    {
        private readonly DTE _dte;
        private readonly JoinableTaskFactory _joinableTaskFactory;

        public VisualStudioSolutionBuilder(DTE dte, JoinableTaskFactory joinableTaskFactory)
        {
            _dte = dte;
            _joinableTaskFactory = joinableTaskFactory;
        }

        public void BuildSolution(string solutionPath)
        {
            _joinableTaskFactory.Run(async () =>
            {
                await _joinableTaskFactory.SwitchToMainThreadAsync();
                _dte.Solution.SolutionBuild.Build(true);
            });
        }
    }
}
