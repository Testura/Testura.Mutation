using System;

namespace Cama.Core.Models.Solution
{
    public class SolutionProjectInfo
    {
        public SolutionProjectInfo(Guid id, string name)
        {
            Id = id;
            Name = name;
        }

        public Guid Id { get; set; }

        public string Name { get; set; }
    }
}
