using System.Collections.Generic;

namespace Cama.Core.Models.Project
{
    public class CamaFileConfig
    {
        public CamaFileConfig()
        {
            TestRunInstancesCount = 3;
            BuildConfiguration = "debug";
        }

        public string SolutionPath { get; set; }

        public IList<string> TestProjects { get; set; }

        public IList<string> MutationProjects { get; set; }

        public IList<string> Filter { get; set; }

        public int TestRunInstancesCount { get; set; }

        public string BuildConfiguration { get; set; }
    }
}
