using System.Collections.Generic;

namespace Cama.Core.Models.Project
{
    public class CamaLocalConfig
    {
        public string SolutionPath { get; set; }

        public IList<string> TestProjectNames { get; set; }

        public IList<string> MutationProjectNames { get; set; }
    }
}
