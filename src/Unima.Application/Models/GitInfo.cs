namespace Unima.Application.Models
{
    public class GitInfo
    {
        public string Url { get; set; }

        public string Branch { get; set; }

        public string Username { get; set; }

        public string Password { get; set; }

        public bool ForceClone { get; set; }

        public bool GenerateFilterFromDiffWithMaster { get; set; }
    }
}
