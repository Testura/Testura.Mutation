namespace Testura.Mutation.Core.Git
{
    public interface IGitDiff
    {
        string GetDiff(string path);
    }
}
