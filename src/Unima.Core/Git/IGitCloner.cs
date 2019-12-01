namespace Unima.Core.Git
{
    public interface IGitCloner
    {
        void ClonseSolution(
            string repositoryUrl,
            string branch,
            string outputPath,
            string username,
            string password);
    }
}
