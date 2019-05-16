using System.Collections.Generic;
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
    }
}
