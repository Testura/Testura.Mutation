using System.Collections.Generic;

namespace Cama.Core.Models.Project
{
    public class CamaFileConfig
    {
        public string SolutionPath { get; set; }

        public IList<string> TestProjects { get; set; }

        public IList<string> MutationProjects { get; set; }

        public IList<string> Filter { get; set; }
    }
}
