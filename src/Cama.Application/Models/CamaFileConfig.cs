using System.Collections.Generic;
using Cama.Core.Creator.Filter;

namespace Cama.Application.Models
{
    public class CamaFileConfig
    {
        public CamaFileConfig()
        {
            NumberOfTestRunInstances = 3;
            MaxTestTimeMin = 5;
            BuildConfiguration = "debug";
            TestRunner = "dotnet";
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
