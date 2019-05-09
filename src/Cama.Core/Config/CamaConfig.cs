using System.Collections.Generic;
using Cama.Core.Baseline;
using Cama.Core.Creator.Filter;
using Cama.Core.Solution;

namespace Cama.Core.Config
{
    public class CamaConfig
    {
        public CamaConfig()
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
    }
}
