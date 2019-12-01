using System.Collections.Generic;
using Unima.Core;
using Unima.Core.Creator.Filter;

namespace Unima.Application.Models
{
    public class UnimaFileConfig
    {
        public UnimaFileConfig()
        {
            NumberOfTestRunInstances = 3;
            MaxTestTimeMin = 5;
            BuildConfiguration = "debug";
            Mutators = new List<string>();
        }

        public string SolutionPath { get; set; }

        public IList<string> TestProjects { get; set; }

        public IList<string> IgnoredProjects { get; set; }

        public MutationDocumentFilter Filter { get; set; }

        public int NumberOfTestRunInstances { get; set; }

        public string BuildConfiguration { get; set; }

        public int MaxTestTimeMin { get; set; }

        public string TestRunner { get; set; }

        public string DotNetPath { get; set; }

        public List<MutationRunLogger> MutationRunLoggers { get; set; }

        public List<string> Mutators { get; set; }

        public GitInfo Git { get; set; }
    }
}
