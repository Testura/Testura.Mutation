using System.Collections.Generic;

namespace Cama.Application.Models
{
    public class CamaFileConfig
    {
        public CamaFileConfig()
        {
            NumberOfTestRunInstances = 3;
            MaxTestTimeMin = 5;
            BuildConfiguration = "debug";
            TestRunner = "nunit";
        }

        public string SolutionPath { get; set; }

        public IList<string> TestProjects { get; set; }

        public IList<string> IgnoredMutationProjects { get; set; }

        public IList<string> Filter { get; set; }

        public int NumberOfTestRunInstances { get; set; }

        public string BuildConfiguration { get; set; }

        public int MaxTestTimeMin { get; set; }

        public string TestRunner { get; set; }
    }
}
