namespace Testura.Mutation.Application.Models
{
    public class GitInfo
    {
        public string RepositoryUrl { get; set; }

        public string Branch { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool ForceClone { get; set; }

        public bool GenerateFilterFromDiffWithMaster { get; set; }

        public string LocalPath { get; set; }
    }
}
