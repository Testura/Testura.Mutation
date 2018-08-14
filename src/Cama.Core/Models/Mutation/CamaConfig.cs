using System.Collections.Generic;
using Newtonsoft.Json;

namespace Cama.Core.Models.Mutation
{
    public class CamaConfig
    {
        public string SolutionPath { get; set; }

        public string TestProjectName { get; set; }

        public IList<string> MutationProjectNames { get; set; }

        [JsonIgnore]
        public string TestProjectOutputPath { get; set; }

        [JsonIgnore]
        public string TestProjectOutputFileName { get; set; }

        [JsonIgnore]
        public string MutationProjectOutputFileName { get; set; }
    }
}
