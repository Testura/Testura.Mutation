using System.Collections.Generic;
using Testura.Mutation.Core.Solution;

namespace Testura.Mutation.Core.Config
{
    public class MutationProject
    {
        public SolutionProjectInfo Project { get; set; }

        public List<string> MappedTestProjects { get; set; }
    }
}
