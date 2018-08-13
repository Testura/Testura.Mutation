using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cama.Core.Models.Mutation
{
    public class CamaConfig
    {
        public string ProjectName { get; set; }

        [JsonIgnore]
        public string ProjectPath { get; set; }

        public string SolutionPath { get; set; }

        public string TestProjectName { get; set; }

        [JsonIgnore]
        public string TestProjectOutputPath { get; set; }

        public IList<string> MutationProjectNames { get; set; }
    }
}
