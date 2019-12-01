namespace Unima.Core.Git
{
    public interface IGitCloner
    {
        void CloneProject(string repositoryUrl, string branch, string username, string password, string outputPath);
    }
}
