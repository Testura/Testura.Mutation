using System.Collections.Generic;

namespace Cama.Core.Models.Project
{
    public class CamaConfig
    {
        public CamaConfig()
        {
            MutationProjects = new List<MutationProjectInfo>();
            TestProjects = new List<TestProjectInfo>();
            Filter = new List<string>();
            MaxTestTimeMin = 5;
        }

        public string SolutionPath { get; set; }

        public IList<MutationProjectInfo> MutationProjects { get; set; }

        public IList<TestProjectInfo> TestProjects { get; set; }

        public IList<string> Filter { get; set; }

        public int TestRunInstancesCount { get; set; }

        public string BuildConfiguration { get; set; }

        public int MaxTestTimeMin { get; set; }
    }
}
