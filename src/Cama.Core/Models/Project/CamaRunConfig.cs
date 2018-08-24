using System.Collections.Generic;

namespace Cama.Core.Models.Project
{
    public class CamaRunConfig
    {
        public CamaRunConfig()
        {
            MutationProjects = new List<MutationProjectInfo>();
            TestProjects = new List<TestProjectInfo>();
        }

        public string SolutionPath { get; set; }

        public IList<MutationProjectInfo> MutationProjects { get; set; }

        public IList<TestProjectInfo> TestProjects { get; set; }
    }
}
