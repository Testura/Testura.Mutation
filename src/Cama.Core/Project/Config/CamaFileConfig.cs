using System.Collections.Generic;

namespace Cama.Core.Config
{
    public class CamaFileConfig
    {
        public CamaFileConfig()
        {
            TestRunInstancesCount = 3;
            MaxTestTimeMin = 5;
            BuildConfiguration = "debug";
        }

        public string SolutionPath { get; set; }

        public IList<string> TestProjects { get; set; }

        public IList<string> IgnoredProjects { get; set; }

        public IList<string> Filter { get; set; }

        public int TestRunInstancesCount { get; set; }

        public string BuildConfiguration { get; set; }

        public int MaxTestTimeMin { get; set; }
    }
}
