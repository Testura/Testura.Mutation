using Testura.Mutation.Application.Models;

namespace Testura.Mutation.Wpf.Helpers.Openers.Tabs
{
    public interface IStartModuleTabOpener
    {
        void OpenNewProjectTab();

        void OpenNewProjectFromGitTab();

        void OpenNewProjectTab(GitInfo gitInfo, string solutionPath);
    }
}
