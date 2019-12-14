using System;
using EnvDTE;
using Unima.Core.Solution;

namespace Unima.VsExtension.Solution
{
    public class VisualStudioSolutionBuilder : ISolutionBuilder
    {
        private readonly DTE _dte;

        public VisualStudioSolutionBuilder(DTE dte)
        {
            _dte = dte;
        }

        public void BuildSolution(string solutionPath)
        {
            _dte.Solution.SolutionBuild.Build(true);
        }
    }
}
