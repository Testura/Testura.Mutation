using System.Collections.Generic;
using Unima.Core.Baseline;
using Unima.Core.Creator.Filter;
using Unima.Core.Creator.Mutators;
using Unima.Core.Solution;

namespace Unima.Core.Config
{
    public class UnimaConfig
    {
        public UnimaConfig()
        {
            MutationProjects = new List<SolutionProjectInfo>();
            TestProjects = new List<TestProject>();
            MaxTestTimeMin = 5;
            BaselineInfos = new List<BaselineInfo>();
        }

        public string SolutionPath { get; set; }

        public IList<SolutionProjectInfo> MutationProjects { get; set; }

        public IList<TestProject> TestProjects { get; set; }

        public MutationDocumentFilter Filter { get; set; }

        public int NumberOfTestRunInstances { get; set; }

        public string BuildConfiguration { get; set; }

        public int MaxTestTimeMin { get; set; }

        public IList<BaselineInfo> BaselineInfos { get; set; }

        public string DotNetPath { get; set; }

        public IList<MutationRunLogger> MutationRunLoggers { get; set; }

        public IList<IMutator> Mutators { get; set; }

        public TargetFramework TargetFramework { get; set; }
    }
}
